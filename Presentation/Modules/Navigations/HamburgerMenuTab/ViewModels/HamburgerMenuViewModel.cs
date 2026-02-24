using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Toolkit.Services;

namespace Aksl.Modules.HamburgerMenuTab.ViewModels
{
    public class HamburgerMenuViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService;
        private readonly IMenuService _menuService;
        private MenuItem _rootMenuItem;
        private MenuItem _parentMenuItem;
        #endregion

        #region Constructors
        public HamburgerMenuViewModel(IEventAggregator eventAggregator, IMenuService menuService, MenuItem rootMenuItem)
        {
            _eventAggregator = eventAggregator;
            _menuService= menuService;

            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            _rootMenuItem = rootMenuItem;

            HamburgerMenuItems = new();

            RegisterActiveTabItemEvent();
            RegisterOnSelectedTabItemEmptyEvent();
        }
        #endregion

        #region Properties
        public ObservableCollection<HamburgerMenuItemViewModel> HamburgerMenuItems { get; private set; }

        private HamburgerMenuItemViewModel _selectedHamburgerMenuItem;
        public HamburgerMenuItemViewModel SelectedHamburgerMenuItem
        {
            get => _selectedHamburgerMenuItem;
            set
            {
                var previewSelectedHamburgerMenuItem = _selectedHamburgerMenuItem;

                if (SetProperty(ref _selectedHamburgerMenuItem, value))
                {
                    if (previewSelectedHamburgerMenuItem is not null && previewSelectedHamburgerMenuItem.IsSelected)
                    {
                        previewSelectedHamburgerMenuItem.IsSelected = false;
                    }

                    if (_selectedHamburgerMenuItem is not null && !_selectedHamburgerMenuItem.IsSelected)
                    {
                        _selectedHamburgerMenuItem.IsSelected = true;
                    }
                }
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (SetProperty<bool>(ref _isPaneOpen, value))
                {
                    foreach (var hmi in HamburgerMenuItems)
                    {
                        hmi.IsPaneOpen = value;
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Register SelectedTabItem Empty Event
        private void RegisterOnSelectedTabItemEmptyEvent()
        {
            _eventAggregator.GetEvent<OnSelectedTabItemEmptyEvent>().Subscribe(async (oatie) =>
            {
                try
                {
                    SelectedHamburgerMenuItem=null;
                }
                catch (Exception ex)
                {
                    await _dialogViewService.AlertAsync(message: $"Exception : \"{ex.Message}\"", title: "Error: Selected TabItem Is Empty");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Register Active TabItem Event
        private void RegisterActiveTabItemEvent()
        {
            _eventAggregator.GetEvent<OnActiveTabItemEvent>().Subscribe(async (oatie) =>
            {
                var currentTabItem = oatie.SelectedTabItem;

                try
                {
                    SetSelectedHamburgerMenuItem();

                    #region Set Selected HamburgerMenuItem Method
                    void SetSelectedHamburgerMenuItem()
                    {
                        var hamburgerMenuItemViewModel = HamburgerMenuItems.FirstOrDefault(hmi => hmi.Name.Equals(currentTabItem.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                                                                                  hmi.Title.Equals(currentTabItem.Title, StringComparison.InvariantCultureIgnoreCase));
                        if (hamburgerMenuItemViewModel is not null)
                        {
                            if (hamburgerMenuItemViewModel!= SelectedHamburgerMenuItem)
                            {
                                SelectedHamburgerMenuItem = hamburgerMenuItemViewModel;
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    await _dialogViewService.AlertAsync(message: $"Exception : \"{ex.Message}\"", title: "Error: Active TabItem");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Create HamburgerMenuItem ViewModel Method
        internal async Task CreateHamburgerMenuItemViewModelsAsync()
        {
            IsLoading = true;

            var parentMenuItem = await _menuService.GetMenuAsync(_rootMenuItem.NavigationName);

            _parentMenuItem = parentMenuItem;

            if (HasSubMenu(_parentMenuItem))
            {
                List<MenuItem> allLeafMenuItems = new();
                foreach (var smi in _parentMenuItem.SubMenus)
                {
                    var leafMenuItems =await GetAllLeafMenuItemsAsync(smi);
                    allLeafMenuItems.AddRange(leafMenuItems);
                }

                int index = 0;
                foreach (var smi in allLeafMenuItems)
                {
                    HamburgerMenuItemViewModel hamburgerMenuItemViewModel = new(_eventAggregator, index++, smi);

                    HamburgerMenuItems.Add(hamburgerMenuItemViewModel);
                }
            }

            SetWorkspaceViewEventName();

            void SetWorkspaceViewEventName()
            {
                foreach (var hmi in HamburgerMenuItems)
                {
                    hmi.WorkspaceViewEventName = _rootMenuItem.WorkspaceViewEventName;
                }
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            IsLoading = false;
        }
        #endregion

        #region Get All LeafMenuItems Method
        private async Task<IEnumerable<MenuItem>> GetAllLeafMenuItemsAsync(MenuItem menuItem)
        {
            List<MenuItem> leafMenuItems = new();

            //if (HasSubMenu(menuItem))
            //{
            //    foreach (var smi in menuItem.SubMenus)
            //    {
            //        await RecursiveSubMenuItem(smi);
            //    }
            //}
            //else if (HasNavigationName(menuItem) && IsLeaf(menuItem))
            //{
            //    var root = await _menuService.GetMenuAsync(menuItem.NavigationName);
            //    foreach (var smi in root.SubMenus)
            //    {
            //        await RecursiveSubMenuItem(smi);
            //    }
            //}
            //else
            //{
            //    await RecursiveSubMenuItem(menuItem);
            //}

            await RecursiveSubMenuItemAsync(menuItem);

            async Task RecursiveSubMenuItemAsync(MenuItem currentMenuItem)
            {
                //if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem))
                //{
                //    leafMenuItems.Add(currentMenuItem);
                //}
                if (!AnyEqualsMenuItems(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && !HasNavigationName(currentMenuItem) && HasTitle(currentMenuItem))
                //if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem))))
                {
                    leafMenuItems.Add(currentMenuItem);
                }

               // if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem) && IsLeaf(currentMenuItem))
               if (HasNavigationName(currentMenuItem)&& IsLeaf(currentMenuItem))
                    {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                }

                if (HasSubMenu(currentMenuItem))
                {
                    foreach (var smi in currentMenuItem.SubMenus)
                    {
                        await RecursiveSubMenuItemAsync(smi);
                    }
                }
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsLeaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.Title);

            bool IsNextNavigation(MenuItem mi) => (mi is not null) && mi.IsNextNavigation;

            bool HasNavigationName(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.NavigationName);
            return leafMenuItems;
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Title, menuItem.Title) || IsEqualsNameOrTitle(mi.Name, menuItem.Name));

            return isEquals;
        }

        private bool IsEqualsNameOrTitle(string nameOrTitle, string otherNameOrTitle)
        {
            if (string.IsNullOrEmpty(nameOrTitle) || string.IsNullOrEmpty(otherNameOrTitle))
            {
                return false;
            }

            var isEquals = (!string.IsNullOrEmpty(nameOrTitle) && nameOrTitle.Equals(otherNameOrTitle, StringComparison.InvariantCultureIgnoreCase)) ||
                           (!string.IsNullOrEmpty(otherNameOrTitle) && otherNameOrTitle.Equals(nameOrTitle, StringComparison.InvariantCultureIgnoreCase));

            return isEquals;
        }
        #endregion
    }
}
