using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels
{
    public class HamburgerMenuSideBarViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarViewModel(IEventAggregator eventAggregator,IMenuService menuService)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            AllLeafHamburgerMenuSideBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> AllLeafHamburgerMenuSideBarItems { get; private set; }
        public string WorkspaceViewEventName { get; set; }

        private HamburgerMenuSideBarItemViewModel _previewSelectedHamburgerMenuItem;
        internal HamburgerMenuSideBarItemViewModel PreviewSelectedHamburgerMenuItem => _previewSelectedHamburgerMenuItem;

        internal HamburgerMenuSideBarItemViewModel _selectedHamburgerMenuSideBarItem;
        public HamburgerMenuSideBarItemViewModel SelectedHamburgerMenuSideBarItem
        {
            get => _selectedHamburgerMenuSideBarItem;
            set
            {
                _previewSelectedHamburgerMenuItem = _selectedHamburgerMenuSideBarItem;

                var previewSelectedHamburgerMenuItem = _selectedHamburgerMenuSideBarItem;

                if (SetProperty(ref _selectedHamburgerMenuSideBarItem, value))
                {
                    if (previewSelectedHamburgerMenuItem is not null && previewSelectedHamburgerMenuItem.IsSelected)
                    {
                        previewSelectedHamburgerMenuItem.IsSelected = false;
                    }

                    if (_selectedHamburgerMenuSideBarItem is not null && !_selectedHamburgerMenuSideBarItem.IsSelected)
                    {
                        _selectedHamburgerMenuSideBarItem.IsSelected = true;
                    }
                }
            }
        }

        public PopupViewModelPair NowPopupViewModelPair { get; set; }
       
        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                if (SetProperty<bool>(ref _isPaneOpen, value))
                {
                    foreach (var hmbi in AllLeafHamburgerMenuSideBarItems)
                    {
                        hmbi.IsPaneOpen = value;
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

        #region Create HamburgerMenuItemBar ViewModel Method
        internal async Task CreateHamburgerMenuBarItemViewModelsAsync()
        {
            IsLoading = true;

            var rootMenuItem = await _menuService.GetMenuAsync("All");

            var subMenuItems = rootMenuItem.SubMenus;
            foreach (var smi in subMenuItems)
            {
                List<MenuItem> travelMenuItems = new();
                var allLeafHierarchicalMenuItemViewModels = await GetAllLeafHamburgerMenuSideBarItemViewModels(smi, travelMenuItems);
                AllLeafHamburgerMenuSideBarItems.AddRange(allLeafHierarchicalMenuItemViewModels);
            }

            var allDistinctLeafHamburgerMenuSideBarItems = AllLeafHamburgerMenuSideBarItems.DistinctBy(item => (item.Name, item.Title));
            AllLeafHamburgerMenuSideBarItems = new ObservableCollection<HamburgerMenuSideBarItemViewModel>(allDistinctLeafHamburgerMenuSideBarItems);

            SetWorkspaceViewEventName();

            void SetWorkspaceViewEventName()
            {
                foreach (var hsmi in AllLeafHamburgerMenuSideBarItems)
                {
                    hsmi.WorkspaceViewEventName = this.WorkspaceViewEventName;

                    AddPropertyChangedOnPopupIsOpen(hsmi);
                }
            }


            void AddPropertyChangedOnPopupIsOpen(HamburgerMenuSideBarItemViewModel hamburgerMenuSideBarItemViewModel)
            {
                hamburgerMenuSideBarItemViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is HamburgerMenuSideBarItemViewModel hmbvm)
                    {
                        if (e.PropertyName == nameof(HamburgerMenuSideBarItemViewModel.ThePopupViewModelPair))
                        {
                            if (NowPopupViewModelPair is null)
                            {
                                NowPopupViewModelPair = hmbvm.ThePopupViewModelPair;
                            }

                            if (NowPopupViewModelPair is not null && NowPopupViewModelPair != hmbvm.ThePopupViewModelPair)
                            {
                                var previewPopupViewModelPair = NowPopupViewModelPair;

                                if (!previewPopupViewModelPair.ThisPopupViewModel.IsOpen && previewPopupViewModelPair.SelectedPopupSideBarItem is not null &&
                                     hmbvm.ThePopupViewModelPair.ThisPopupViewModel.IsOpen && hmbvm.ThePopupViewModelPair.SelectedPopupSideBarItem is not null && 
                                     previewPopupViewModelPair.SelectedPopupSideBarItem!= hmbvm.ThePopupViewModelPair.SelectedPopupSideBarItem)

                                {
                                    previewPopupViewModelPair.ThisPopupViewModel.ClearSelectedPopupSideBarItems();
                                }

                                NowPopupViewModelPair = hmbvm.ThePopupViewModelPair;
                            }
                        }
                    }
                };
            }

            IsLoading = false;
        }
        #endregion

        #region Get Top Leaf HamburgerMenuBarItemViewModel Method
        internal IEnumerable<HamburgerMenuSideBarItemViewModel> GetTopLeafHamburgerMenuSideBarItemViewModels(HamburgerMenuSideBarItemViewModel topHamburgerMenuSideBarItemViewModel)
        {
            List<HamburgerMenuSideBarItemViewModel> topLeafHamburgerMenuSideBarItemViewModels = new();

            RecursiveSubMenuItemViewModel(topHamburgerMenuSideBarItemViewModel);

            void RecursiveSubMenuItemViewModel(HamburgerMenuSideBarItemViewModel currenyHamburgerMenuSideBarItemViewModel)
            {
                if (!AnyEqualsHamburgerMenuSideBarItemViewModels(topLeafHamburgerMenuSideBarItemViewModels, currenyHamburgerMenuSideBarItemViewModel) && currenyHamburgerMenuSideBarItemViewModel.IsLeaf && currenyHamburgerMenuSideBarItemViewModel.HasTitle)
                {
                    topLeafHamburgerMenuSideBarItemViewModels.Add(currenyHamburgerMenuSideBarItemViewModel);
                }

                if (HasChild(currenyHamburgerMenuSideBarItemViewModel))
                {
                    foreach (var children in currenyHamburgerMenuSideBarItemViewModel.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HamburgerMenuSideBarItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return topLeafHamburgerMenuSideBarItemViewModels;
        }
        #endregion

        #region Get All Leaf HamburgerMenuSideBarItemViewModels Method
        internal async Task<IEnumerable<HamburgerMenuSideBarItemViewModel>> GetAllLeafHamburgerMenuSideBarItemViewModels(MenuItem menuItem,IList<MenuItem> travelMenuItems)
        {
            List<HamburgerMenuSideBarItemViewModel> leafHamburgerMenuSideBarItemViewModels = new();

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                var isAddOnLeaf = IsLeaf(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem)));
                var isAddOnNotLeaf = !IsLeaf(currentMenuItem) && !IsNexOnNotLeaf(currentMenuItem);
                //if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem))))
                if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem) && HasTitle(currentMenuItem) && (isAddOnLeaf || isAddOnNotLeaf))
                {
                    travelMenuItems.Add(currentMenuItem);
                    leafHamburgerMenuSideBarItemViewModels.Add(new(currentMenuItem, null));

                    //leafHamburgerMenuSideBarItemViewModels.Add(new(_eventAggregator, currentMenuItem));
                }

                //  if (HasNavigationName(currentMenuItem) && IsLeaf(currentMenuItem))
                //if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem) && IsLeaf(currentMenuItem))
                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem))
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                }

                //if (HasSubMenu(currentMenuItem))
                if (HasSubMenu(currentMenuItem) && IsNexOnNotLeaf(currentMenuItem))
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

            bool IsNexOnNotLeaf(MenuItem mi) => (mi is not null) && mi.IsNexOnNotLeaf;

            return leafHamburgerMenuSideBarItemViewModels;
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsHamburgerMenuSideBarItemViewModels(IEnumerable<HamburgerMenuSideBarItemViewModel> hamburgerMenuSideBarItemViewModels, HamburgerMenuSideBarItemViewModel hamburgerMenuSideBarItemViewModel)
        {
            var isExists = hamburgerMenuSideBarItemViewModels.Any(hmivm => IsEqualsNameOrTitle(hmivm.Name, hamburgerMenuSideBarItemViewModel.Name) || IsEqualsNameOrTitle(hmivm.Title, hamburgerMenuSideBarItemViewModel.Title));

            return isExists;
        }

        private bool AnyEqualsMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi =>IsEqualsNameOrTitle(mi.Name, menuItem.Name) || IsEqualsNameOrTitle(mi.Title, menuItem.Title)  );

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
