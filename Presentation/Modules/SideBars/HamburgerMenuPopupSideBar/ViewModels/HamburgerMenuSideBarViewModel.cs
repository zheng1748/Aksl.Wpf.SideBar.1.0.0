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
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarViewModel(IMenuService menuService)
        {
            _menuService = menuService;

            AllLeafHamburgerMenuSideBarItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> AllLeafHamburgerMenuSideBarItems { get; private set; }
        public string WorkspaceViewEventName { get; set; }

        internal HamburgerMenuSideBarItemViewModel _selectedHamburgerMenuSideBarItem;
        public HamburgerMenuSideBarItemViewModel SelectedHamburgerMenuSideBarItem
        {
            get => _selectedHamburgerMenuSideBarItem; 
            set => SetProperty(ref _selectedHamburgerMenuSideBarItem, value);
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
                var allLeafsOfMenuItem = await GetLeafsOfMenuItem(smi);
                AllLeafHamburgerMenuSideBarItems.AddRange(allLeafsOfMenuItem);
            }

            var allDistinctLeafs = AllLeafHamburgerMenuSideBarItems.DistinctBy(item => (item.Name, item.Title));
            AllLeafHamburgerMenuSideBarItems = new ObservableCollection<HamburgerMenuSideBarItemViewModel>(allDistinctLeafs);

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

                                SetSelectedHamburgerMenuSideBarItem(hmbvm.ThePopupViewModelPair.HamburgerMenuSideBarItem);
                                NowPopupViewModelPair = hmbvm.ThePopupViewModelPair;
                            }
                        }
                    }
                };
            }

            IsLoading = false;
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

        #region Set Selected Method
        public void SetSelectedHamburgerMenuSideBarItem(HamburgerMenuSideBarItemViewModel hamburgerMenuSideBarItemViewModel)
        {
            var selectedHamburgerMenuSideBarItem = AllLeafHamburgerMenuSideBarItems.FirstOrDefault(hsmi =>  hsmi.IsSelected);
            if (selectedHamburgerMenuSideBarItem is not null && hamburgerMenuSideBarItemViewModel != selectedHamburgerMenuSideBarItem)
            {
                selectedHamburgerMenuSideBarItem.IsSelected = false;
                hamburgerMenuSideBarItemViewModel.IsSelected = true;
            }
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
