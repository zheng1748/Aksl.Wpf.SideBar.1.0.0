using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.ExpandHamburgerMenuSub.ViewModels
{
    public class HamburgerMenuViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        private MenuItem _rootMenuItem;
        private MenuItem _parentMenuItem;
        #endregion

        #region Constructors
        public HamburgerMenuViewModel(IEventAggregator eventAggregator, IMenuService menuService, MenuItem rootMenuItem)
        {
            _eventAggregator = eventAggregator;
            _menuService= menuService;

            _rootMenuItem = rootMenuItem;

            HamburgerMenuItems = new();
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
                    var leafMenuItems = await GetAllLeafMenuItems(smi);
                    allLeafMenuItems.AddRange(leafMenuItems);
                }

                int index = 0;
                foreach (var smi in allLeafMenuItems)
                {
                    HamburgerMenuItemViewModel hamburgerMenuItemViewModel = new(_eventAggregator, index++, smi);
                    //AddPropertyChanged();

                    //void AddPropertyChanged()
                    //{
                    //    hamburgerMenuItemViewModel.PropertyChanged += (sender, propertyName) =>
                    //    {
                    //        if (sender is  HamburgerMenuItemViewModel nivm)
                    //        {
                    //            if (!nivm.IsSelected)
                    //            {
                    //                _selectedNavigationItem =null;
                    //                RaisePropertyChanged(nameof(SelectedNavigationItem));
                    //            }
                    //        }
                    //    };
                    //}
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
        private async Task<IEnumerable<MenuItem>> GetAllLeafMenuItems(MenuItem menuItem)
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

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                //if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem))
                //{
                //    leafMenuItems.Add(currentMenuItem);
                //}
                //if (!AnyEqualsMenuItem(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && !HasNavigationName(currentMenuItem) && HasTitle(currentMenuItem))
                if (!AnyEqualsMenuItems(leafMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem))))
                {
                    leafMenuItems.Add(currentMenuItem);
                }

                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem))
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                }

                if (HasSubMenu(currentMenuItem))
                {
                    foreach (var smi in currentMenuItem.SubMenus)
                    {
                       await RecursiveSubMenuItem(smi);
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
