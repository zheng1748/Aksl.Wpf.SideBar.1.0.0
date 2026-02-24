using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.ExpandHamburgerMenuNavigationBar.ViewModels
{
    public class GroupedMenusViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        private MenuItem _rootMenuItem;
        private MenuItem _parentMenuItem;
        private int _currentGroupeIndex = -1;
        #endregion

        #region Constructors
        public GroupedMenusViewModel(IEventAggregator eventAggregator, IMenuService menuService, MenuItem rootMenuItem)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            _rootMenuItem = rootMenuItem;

            GroupedMenus = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<GroupedMenuViewModel> GroupedMenus { get; }

       // private MenuItemViewModel _selectedMenuItemItem;
        public MenuItemViewModel SelectedMenuItem { get; set; }
        //{
        //    get => _selectedMenuItemItem;
        //    set
        //    {
        //        if (SetProperty(ref _selectedMenuItemItem, value))
        //        {
        //            if (_selectedMenuItemItem is not null)
        //            {
        //            }
        //        }
        //    }
        //}

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create GroupedMenu ViewModels Method
        internal async Task CreateGroupedMenuViewModelsAsync()
        {
            IsLoading = true;

            var parentMenuItem = await _menuService.GetMenuAsync(_rootMenuItem.NavigationName);

            _parentMenuItem = parentMenuItem;

            int index = 0;
            foreach (var smi in _parentMenuItem.SubMenus)
            {
                var leafMenuItems =await GetAllLeafMenuItems(smi);

                GroupedMenuViewModel groupedMenuViewModel = new(_eventAggregator, index++, smi, leafMenuItems);
                AddPropertyChanged();

                GroupedMenus.Add(groupedMenuViewModel);

                void AddPropertyChanged()
                {
                    groupedMenuViewModel.PropertyChanged += (sender, e) =>
                    {
                        if (sender is GroupedMenuViewModel gmvm)
                        {
                            if (e.PropertyName == nameof(GroupedMenuViewModel.IsLoading))
                            {
                                //最后一个
                                if (gmvm.GroupIndex == GroupedMenus.Count() && !gmvm.IsLoading)
                                {
                                    IsLoading = false;
                                }
                            }

                            if (e.PropertyName == nameof(GroupedMenuViewModel.MenuContent))
                            {
                                if (_currentGroupeIndex == gmvm.GroupIndex)
                                {
                                    SelectedMenuItem = gmvm.MenuContent.SelectedMenuItem;
                                }
                                else
                                {
                                    foreach (var gm in GroupedMenus)
                                    {
                                        if (_currentGroupeIndex == gm.GroupIndex)
                                        {
                                            gm.MenuContent.ClearSelectedMenuItem();
                                        }
                                    }

                                    _currentGroupeIndex = gmvm.GroupIndex;
                                    SelectedMenuItem = gmvm.MenuContent.SelectedMenuItem;
                                }
                            }
                        }
                    };
                }
            }

            SetWorkspaceViewEventName();

            void SetWorkspaceViewEventName()
            {
                foreach (var gm in GroupedMenus)
                {
                    foreach (var mi in gm.MenuContent.MenuItems)
                    {
                        mi.WorkspaceViewEventName = _rootMenuItem.WorkspaceViewEventName;
                    }
                }
            }

            IsLoading = false;
        }
        #endregion

        #region Get All LeafMenuItems Method
        private async Task<IEnumerable<MenuItem>> GetAllLeafMenuItems(MenuItem menuItem)
        {
            List<MenuItem> leafMenuItems = new();

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                var isAddOnLeaf = IsLeaf(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem)));
                var isAddOnNotLeaf = !IsLeaf(currentMenuItem) && !IsNexOnNotLeaf(currentMenuItem);
                if (!AnyEqualsMenuItems(leafMenuItems, currentMenuItem) && HasTitle(currentMenuItem) && (isAddOnLeaf || isAddOnNotLeaf))
                {
                    leafMenuItems.Add(currentMenuItem);
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
