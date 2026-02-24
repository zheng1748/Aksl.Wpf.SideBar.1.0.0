using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.ExpandHamburgerMenuTreeBar.ViewModels
{
    public class TreeBarViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        private MenuItem _rootMenuItem;
        private MenuItem _parentMenuItem;
        #endregion

        #region Constructors
        public TreeBarViewModel(IEventAggregator eventAggregator, IMenuService menuService, MenuItem rootMenuItem)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            _rootMenuItem = rootMenuItem;

            //TreeBarItems = new();
            AllTreeBarItems = new();
        }
        #endregion

        #region Properties
        //public ObservableCollection<TreeBarItemViewModel> TreeBarItems { get; }

        public ObservableCollection<TreeBarItemViewModel> AllTreeBarItems { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create TreeBarItem ViewModel Method
        internal async Task CreateTreeBarItemViewModelsAsync()
        {
            IsLoading = true;

            var parentMenuItem = await _menuService.GetMenuAsync(_rootMenuItem.NavigationName);

            _parentMenuItem = parentMenuItem;

            foreach (var smi in _parentMenuItem.SubMenus)
            {
                //TreeBarItemViewModel treeBarItemViewModel = new(_eventAggregator, smi);
                //TreeBarItems.Add(treeBarItemViewModel);

                //TreeBarItemViewModel parent = new(_eventAggregator, smi);
                //AllTreeBarItems.Add(parent);
                //List<MenuItem> allTravelMenuItems = new();
                //await GetAllTreeBarItemSubViewModelsAsync(smi, allTravelMenuItems, parent);

                List<MenuItem> allTravelMenuItems = new();
                var topTreeBarItemViewModel = await GetAllTreeBarItemViewModelsByMenuItem(smi, allTravelMenuItems);
                AllTreeBarItems.Add(topTreeBarItemViewModel);
            }

            //var allLeafHierarchicalSubMenuItemViewModels = GetAllLeafHierarchicalSubMenuItemViewModels();
            //AllLeafHierarchicalSubMenuItems = new(allLeafHierarchicalSubMenuItemViewModels.AllLeaf);

            SetWorkspaceViewEventName();

            void SetWorkspaceViewEventName()
            {
                foreach (var tbi in AllTreeBarItems)
                {
                    RecursiveSubMenuItem(tbi);
                }

                void RecursiveSubMenuItem(TreeBarItemViewModel treeBarItemViewModel)
                {
                    if (treeBarItemViewModel.IsLeaf)
                    {
                        treeBarItemViewModel.WorkspaceViewEventName = _rootMenuItem.WorkspaceViewEventName;
                    }

                    if (HasChild(treeBarItemViewModel))
                    {
                        foreach (var smi in treeBarItemViewModel.Children)
                        {
                            RecursiveSubMenuItem(smi);
                        }
                    }
                }
            }

            bool HasChild(TreeBarItemViewModel tbivm) => (tbivm is not null) && tbivm.Children.Any();

            IsLoading = false;
        }
        #endregion

        #region Get All TreeBarItemViewModels Method
        internal async Task<TreeBarItemViewModel> GetAllTreeBarItemViewModelsByMenuItem(MenuItem menuItem, IList<MenuItem> travelMenuItems)
        {
            TreeBarItemViewModel virtualParent = new();

            await RecursiveSubMenuItem(menuItem, virtualParent);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem, TreeBarItemViewModel paren)
            {
                TreeBarItemViewModel child = default;

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

            var child = virtualParent.Children.FirstOrDefault();
            if (child is not null)
            {
                child.Parent = null;
            }
            return child;
        }
        #endregion

        #region Get All TreeBarItem SubViewModels Methods
        private async Task GetAllTreeBarItemSubViewModelsAsync(MenuItem menuItem, IList<MenuItem> travelMenuItems, TreeBarItemViewModel currentTreeBarItemViewModel)
        {
            #region Method

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem))
                {
                    travelMenuItems.Add(currentMenuItem);
                }

                var matchResult = FindMatchTreeBarItemViewModelByMenuItem(currentTreeBarItemViewModel, currentMenuItem);
                Debug.Assert(matchResult.IsTrue);
                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem) && IsLeaf(currentMenuItem) && matchResult.FindTreeBarItemViewModel.IsLeaf)
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);

                    if (HasSubMenu(currentMenuItem))
                    {
                        var parent = matchResult.FindTreeBarItemViewModel;

                        foreach (var smi in currentMenuItem.SubMenus)
                        {
                            TreeBarItemViewModel barItemViewModel = new(_eventAggregator, smi, parent);
                            parent.Children.Add(barItemViewModel);
                        }
                    }
                }

                if (HasSubMenu(currentMenuItem))
                {
                    foreach (var smi in currentMenuItem.SubMenus)
                    {
                        await RecursiveSubMenuItem(smi);
                    }
                }
            }
            #endregion

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsLeaf(MenuItem mi) => (mi is not null) && mi.SubMenus.Count <= 0;

            bool IsNextNavigation(MenuItem mi) => (mi is not null) && mi.IsNextNavigation;

            bool HasNavigationName(MenuItem mi) => (mi is not null) && !string.IsNullOrEmpty(mi.NavigationName);
        }

        private (TreeBarItemViewModel FindTreeBarItemViewModel, bool IsTrue) FindMatchTreeBarItemViewModelByMenuItem(TreeBarItemViewModel treeBarItemViewModel, MenuItem menuItem)
        {
            //var findViewModel = FindByNameOrTitle(treeBarItemViewModel, menuItem.Name);
            //if (findViewModel is null)
            //{
            //    findViewModel = FindByNameOrTitle(treeBarItemViewModel, menuItem.Title);
            //}

            var findViewModel = FindTreeBarItemViewModelByMenuItem(treeBarItemViewModel, menuItem);

            return (FindTreeBarItemViewModel: findViewModel, IsTrue: (findViewModel is not null));
        }

        private TreeBarItemViewModel FindTreeBarItemViewModelByMenuItem(TreeBarItemViewModel treeBarItemViewModel, MenuItem menuItem)
        {
            TreeBarItemViewModel findTreeBarItemViewModel = null;

            RecursiveSubMenuItemViewModel(treeBarItemViewModel);

            void RecursiveSubMenuItemViewModel(TreeBarItemViewModel parent)
            {
                if (IsEqualsNameOrTitle(parent.Name, menuItem.Name) || IsEqualsNameOrTitle(parent.Title, menuItem.Title))
                {
                    findTreeBarItemViewModel = parent;
                    return;
                }

                if (HasChild(parent))
                {
                    foreach (var children in parent.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(TreeBarItemViewModel tbivm) => (tbivm is not null) && tbivm.Children.Any();

            return findTreeBarItemViewModel;
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Title, menuItem.Title) || IsEqualsNameOrTitle(mi.Name, menuItem.Name));

            return isEquals;
        }

        //private bool NameOrTitlelsEquals(TreeBarItemViewModel treeBarItemViewModel, string nameOrTitle)
        //{
        //    var isEquals = IsEqualsNameOrTitle(treeBarItemViewModel.Name, nameOrTitle) || IsEqualsNameOrTitle(treeBarItemViewModel.Title, nameOrTitle);

        //    return isEquals;
        //}

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
