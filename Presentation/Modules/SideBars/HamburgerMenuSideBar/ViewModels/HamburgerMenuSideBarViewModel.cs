using Aksl.Infrastructure;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Aksl.Modules.HamburgerMenuSideBar.ViewModels
{
    public class HamburgerMenuSideBarViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarViewModel(IEventAggregator eventAggregator, IMenuService menuService)
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
                var allLeafHierarchicalMenuItemViewModels = await GetAllLeafsOfMenuItem(smi);
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
                }
            }

            IsLoading = false;
        }
        #endregion

        #region Get All Leafs Method
        internal async Task<IEnumerable<HamburgerMenuSideBarItemViewModel>> GetAllLeafsOfMenuItem(MenuItem menuItem)
        {
            #region Method
            //List<HamburgerMenuSideBarItemViewModel> leafHamburgerMenuSideBarItemViewModels = new();

            //await RecursiveSubMenuItem(menuItem);

            //async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            //{
            //    var isAddOnLeaf = IsLeaf(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem)));
            //    var isAddOnNotLeaf = !IsLeaf(currentMenuItem) && !IsNexOnNotLeaf(currentMenuItem);
            //    //if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem) && IsLeaf(currentMenuItem) && HasTitle(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem))))
            //    if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem) && HasTitle(currentMenuItem) && (isAddOnLeaf || isAddOnNotLeaf))
            //    {
            //        travelMenuItems.Add(currentMenuItem);
            //        leafHamburgerMenuSideBarItemViewModels.Add(new(currentMenuItem, null));

            //        //leafHamburgerMenuSideBarItemViewModels.Add(new(_eventAggregator, currentMenuItem));
            //    }

            //    //  if (HasNavigationName(currentMenuItem) && IsLeaf(currentMenuItem))
            //    //if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem) && IsLeaf(currentMenuItem))
            //    if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem))
            //    {
            //        currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
            //    }

            //    //if (HasSubMenu(currentMenuItem))
            //    if (HasSubMenu(currentMenuItem) && IsNexOnNotLeaf(currentMenuItem))
            //    {
            //        foreach (var smi in currentMenuItem.SubMenus)
            //        {
            //            await RecursiveSubMenuItem(smi);
            //        }
            //    }
            //}
            #endregion

            List<MenuItem> travelMenuItems = new();
            List<HamburgerMenuSideBarItemViewModel> leafsOfMenuItem = new();
            HamburgerMenuSideBarItemViewModel virtualParent = new();

            await RecursiveSubMenuItem(menuItem, virtualParent);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem, HamburgerMenuSideBarItemViewModel paren)
            {
                HamburgerMenuSideBarItemViewModel child = default;

                if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem))
                {
                    travelMenuItems.Add(currentMenuItem);

                    child = new(currentMenuItem, paren);
                }

                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem))
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                }

                if (HasSubMenu(currentMenuItem) && IsNexOnNotLeaf(currentMenuItem))
                {
                    foreach (var smi in currentMenuItem.SubMenus)
                    {
                        await RecursiveSubMenuItem(smi, child);
                    }
                }
            }

            var topHeaderItem = virtualParent.Children.FirstOrDefault();
            if (topHeaderItem is not null)
            {
                topHeaderItem.Parent = null;

                leafsOfMenuItem = GetAllLeafsOfHeaderItem(topHeaderItem);
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsLeaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.Title);

            bool IsNextNavigation(MenuItem mi) => (mi is not null) && mi.IsNextNavigation;

            bool HasNavigationName(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.NavigationName);

            bool IsNexOnNotLeaf(MenuItem mi) => (mi is not null) && mi.IsNexOnNotLeaf;

            return leafsOfMenuItem;
        }
        #endregion

        #region Get All Leafs Header Method
        private List<HamburgerMenuSideBarItemViewModel> GetAllLeafsOfHeaderItem(HamburgerMenuSideBarItemViewModel topHeaderItem)
        {
            List<HamburgerMenuSideBarItemViewModel> leafsOfTopHeaderItem = new();

            RecursiveSubMenuItemViewModel(topHeaderItem);

            void RecursiveSubMenuItemViewModel(HamburgerMenuSideBarItemViewModel currenySubItem)
            {
                if (!AnyEqualsHamburgerMenuSideBarItemViewModels(leafsOfTopHeaderItem, currenySubItem) && currenySubItem.IsLeaf && currenySubItem.HasTitle)
                {
                    leafsOfTopHeaderItem.Add(currenySubItem);
                }

                if (currenySubItem.HasChildren)
                {
                    foreach (var children in currenySubItem.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            return leafsOfTopHeaderItem;
        }
        #endregion


        #region Contain Methods

        private bool AnyEqualsHamburgerMenuSideBarItemViewModels(IEnumerable<HamburgerMenuSideBarItemViewModel> hamburgerMenuSideBarItemViewModels, HamburgerMenuSideBarItemViewModel hamburgerMenuSideBarItemViewModel)
        {
            if (hamburgerMenuSideBarItemViewModels is null || (hamburgerMenuSideBarItemViewModels is not null && !hamburgerMenuSideBarItemViewModels.Any()) || hamburgerMenuSideBarItemViewModel is null)
            {
                return false;
            }

            var isAny = hamburgerMenuSideBarItemViewModels.Any(hmivm => IsEqualsNameOrTitle(hmivm.Name, hamburgerMenuSideBarItemViewModel.Name) || IsEqualsNameOrTitle(hmivm.Title, hamburgerMenuSideBarItemViewModel.Title));

            return isAny;
        }

        private bool AnyEqualsMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isAny = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Name, menuItem.Name) || IsEqualsNameOrTitle(mi.Title, menuItem.Title));

            return isAny;
        }

        private bool IsEqualsNameOrTitle(string nameOrTitle, string otherNameOrTitle)
        {
            if (string.IsNullOrEmpty(nameOrTitle) || string.IsNullOrEmpty(otherNameOrTitle))
            {
                return false;
            }

            var isAny = (!string.IsNullOrEmpty(nameOrTitle) && nameOrTitle.Equals(otherNameOrTitle, StringComparison.InvariantCultureIgnoreCase)) ||
                        (!string.IsNullOrEmpty(otherNameOrTitle) && otherNameOrTitle.Equals(nameOrTitle, StringComparison.InvariantCultureIgnoreCase));

            return isAny;
        }
        #endregion
    }
}
