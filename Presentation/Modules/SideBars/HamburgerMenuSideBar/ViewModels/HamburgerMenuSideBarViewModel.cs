using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuSideBar.ViewModels
{
    public class HamburgerMenuSideBarViewModel : BindableBase
    {
        #region Members
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarViewModel(IMenuService menuService)
        {
            _menuService = menuService;

            AllLeafHamburgerMenuSideBarItems = new();
        }

        public HamburgerMenuSideBarViewModel()
        {
            AllLeafHamburgerMenuSideBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> AllLeafHamburgerMenuSideBarItems { get; set; }
        public string WorkspaceViewEventName { get; set; }

        //private HamburgerMenuSideBarItemViewModel _previewSelectedHamburgerMenuItem;
        //internal HamburgerMenuSideBarItemViewModel PreviewSelectedHamburgerMenuItem => _previewSelectedHamburgerMenuItem;

        internal HamburgerMenuSideBarItemViewModel _selectedHamburgerMenuSideBarItem;
        public HamburgerMenuSideBarItemViewModel SelectedHamburgerMenuSideBarItem
        {
            get => _selectedHamburgerMenuSideBarItem;
            set
            {
                SetProperty(ref _selectedHamburgerMenuSideBarItem, value);
                //_previewSelectedHamburgerMenuItem = _selectedHamburgerMenuSideBarItem;

                //var previewSelectedHamburgerMenuItem = _selectedHamburgerMenuSideBarItem;

                //if (SetProperty(ref _selectedHamburgerMenuSideBarItem, value))
                //{
                //    if (previewSelectedHamburgerMenuItem is not null && previewSelectedHamburgerMenuItem.IsSelected)
                //    {
                //        previewSelectedHamburgerMenuItem.IsSelected = false;
                //    }

                //    if (_selectedHamburgerMenuSideBarItem is not null && !_selectedHamburgerMenuSideBarItem.IsSelected)
                //    {
                //        _selectedHamburgerMenuSideBarItem.IsSelected = true;
                //    }
                //}
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
                // var allLeafsOfMenuItem = await GetLeafsOfMenuItem(smi);
                //var topHeaderItem = await GetTopHeaderTreeByMenuItemAsync(smi);
                //var allLeafsOfMenuItem = await GetLeafsOfTopHeaderItemAsync(topHeaderItem);
                var allLeafsOfMenuItem = await GetLeafsOfMenuItemInTreeAsync(smi);
                AllLeafHamburgerMenuSideBarItems.AddRange(allLeafsOfMenuItem);
            }

            var allDistinctLeafs = AllLeafHamburgerMenuSideBarItems.DistinctBy(item => (item.Name, item.Title));
            AllLeafHamburgerMenuSideBarItems = new ObservableCollection<HamburgerMenuSideBarItemViewModel>(allDistinctLeafs);

            SetWorkspaceViewEventName();

            IsLoading = false;
        }
        #endregion

        #region Set WorkspaceView EventName Method
        internal void SetWorkspaceViewEventName()
        {
            foreach (var hsmi in AllLeafHamburgerMenuSideBarItems)
            {
                hsmi.WorkspaceViewEventName = this.WorkspaceViewEventName;
            }
        }
        #endregion

        #region Get Leafs Of MenuItem Method
        internal async Task<IEnumerable<HamburgerMenuSideBarItemViewModel>> GetLeafsOfMenuItem(MenuItem menuItem)
        {
            List<MenuItem> travelMenuItems = new();
            List<HamburgerMenuSideBarItemViewModel> leafsOfMenuItem = new();

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                var isAddOnLeaf = IsLeaf(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem)));
                var isAddOnNotLeaf = !IsLeaf(currentMenuItem) && !IsNexOnNotLeaf(currentMenuItem);
                if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem) && HasTitle(currentMenuItem) && (isAddOnLeaf || isAddOnNotLeaf))
                {
                    leafsOfMenuItem.Add(new(currentMenuItem, null));
                    travelMenuItems.Add(currentMenuItem);
                }

                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem))
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);
                }

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

            return leafsOfMenuItem;
        }
        #endregion

        #region Get Leafs Of MenuItem In Tree Method
        internal async Task<IEnumerable<HamburgerMenuSideBarItemViewModel>> GetLeafsOfMenuItemInTreeAsync(MenuItem menuItem)
        {
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

                leafsOfMenuItem =await GetLeafsOfTopHeaderItemAsync(topHeaderItem);
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
        private async Task<HamburgerMenuSideBarItemViewModel> GetTopHeaderTreeByMenuItemAsync(MenuItem menuItem)
        {
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

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsLeaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool HasTitle(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.Title);

            bool IsNextNavigation(MenuItem mi) => (mi is not null) && mi.IsNextNavigation;

            bool HasNavigationName(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.NavigationName);

            bool IsNexOnNotLeaf(MenuItem mi) => (mi is not null) && mi.IsNexOnNotLeaf;

            var topHeaderItem = virtualParent.Children.FirstOrDefault();
            if (topHeaderItem is not null)
            {
                topHeaderItem.Parent = null;
            }
            return topHeaderItem;
        }
        #endregion

        #region Get Leafs Of TopHeaderItem Method
        private async Task<List<HamburgerMenuSideBarItemViewModel>> GetLeafsOfTopHeaderItemAsync(HamburgerMenuSideBarItemViewModel topHeaderItem)
        {
            List<HamburgerMenuSideBarItemViewModel> leafsOfTopHeaderItem = new();

            await RecursiveSubMenuItemViewModel(topHeaderItem);

            async Task RecursiveSubMenuItemViewModel(HamburgerMenuSideBarItemViewModel currenySubItem)
            {
                if (!AnyEqualsHamburgerMenuSideBarItemViewModels(leafsOfTopHeaderItem, currenySubItem) && currenySubItem.IsLeaf && currenySubItem.HasTitle)
                {
                    leafsOfTopHeaderItem.Add(currenySubItem);
                }

                if (currenySubItem.HasChildren)
                {
                    foreach (var children in currenySubItem.Children)
                    {
                        await RecursiveSubMenuItemViewModel(children);
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
