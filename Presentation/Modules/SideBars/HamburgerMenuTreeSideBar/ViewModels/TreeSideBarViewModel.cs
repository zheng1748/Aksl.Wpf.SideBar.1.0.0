using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.HamburgerMenuTreeSideBar.ViewModels
{
    public class TreeSideBarViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public TreeSideBarViewModel(IEventAggregator eventAggregator, IMenuService menuService)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            //TopTreeSideBarItems = new();
            AllTreeSideBarItems = new();
        }
        #endregion

        #region Properties
        //public ObservableCollection<TreeSideBarItemViewModel> TopTreeSideBarItems { get; }
        public ObservableCollection<TreeSideBarItemViewModel> AllTreeSideBarItems { get; }
        public string WorkspaceViewEventName { get; set; }
      
        //internal TreeSideBarItemViewModel _previewSelectedTreeSideBarItem;
        //internal TreeSideBarItemViewModel PreviewSelectedTreeSideBarItem => _previewSelectedTreeSideBarItem;

        private TreeSideBarItemViewModel _selectedTreeSideBarItem;
        public TreeSideBarItemViewModel SelectedTreeSideBarItem
        {
            get => _selectedTreeSideBarItem;
            set
            {
                SetProperty(ref _selectedTreeSideBarItem, value);
                //if (SetProperty(ref _selectedTreeSideBarItem, value))
                //{
                //    if (_selectedTreeSideBarItem is not null)
                //    {
                //        _selectedTreeSideBarItem.IsSelected = true;
                //    }
                //}
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Reset/Clear Selected TreeSideBarItem Method
        internal void ClearSelectedTreeSideBarItem()
        {
            if (SelectedTreeSideBarItem is not null)
            {
                SelectedTreeSideBarItem.IsSelected = false;
                SelectedTreeSideBarItem = null;
                //_previewSelectedTreeSideBarItem = null;
            }
        }

        internal void ResetSelectedTreeSideBarItem(TreeSideBarItemViewModel selectedTreeSideBarItem)
        {
            if (selectedTreeSideBarItem is not null)
            {
                if (_selectedTreeSideBarItem is not null)
                {
                    _selectedTreeSideBarItem.IsSelected = false;
                }

                // _previewSelectedTreeSideBarItem = null;
                _selectedTreeSideBarItem = selectedTreeSideBarItem;
                _selectedTreeSideBarItem.IsSelected = true;
            }
        }
        #endregion

        #region Create TreeSideBarItem ViewModel Method
        internal async Task CreateTreeSideBarItemViewModelsAsync()
        {
            IsLoading = true;

            var rootMenuItem = await _menuService.GetMenuAsync("All");

            var subMenuItems = rootMenuItem.SubMenus;
            foreach (var smi in subMenuItems)
            {
                //TreeSideBarItemViewModel treeSideBarItemViewModel = new(_eventAggregator, smi);
                //TopTreeSideBarItems.Add(treeSideBarItemViewModel);

                //TreeSideBarItemViewModel parent = new(_eventAggregator, smi);
                //AllTreeSideBarItems.Add(parent);
                //List<MenuItem> allTravelMenuItems = new();
                //await GetAllTreeBarItemSubViewModelsAsync(smi, allTravelMenuItems, parent);

                var treeSideBarItemViewModel = await GetAllTreeSideBarItemViewModelsByMenuItem(smi);
                AllTreeSideBarItems.Add(treeSideBarItemViewModel);
            }

            SetWorkspaceViewEventNameAndPropertyChanged();

            void SetWorkspaceViewEventNameAndPropertyChanged()
            {
                foreach (var tbi in AllTreeSideBarItems)
                {
                    RecursiveSubMenuItem(tbi);
                }

                void RecursiveSubMenuItem(TreeSideBarItemViewModel treeSideBarItemViewModel)
                {
                    AddPropertyChanged(treeSideBarItemViewModel);

                    if (treeSideBarItemViewModel.IsLeaf)
                    {
                        treeSideBarItemViewModel.WorkspaceViewEventName = this.WorkspaceViewEventName;
                    }

                    if (treeSideBarItemViewModel.HasChildren)
                    {
                        foreach (var smi in treeSideBarItemViewModel.Children)
                        {
                            RecursiveSubMenuItem(smi);
                        }
                    }
                }
            }

            void AddPropertyChanged(TreeSideBarItemViewModel treeSideBarItemViewModel)
            {
                treeSideBarItemViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is TreeSideBarItemViewModel tsbivm)
                    {
                        if (e.PropertyName == nameof(TreeSideBarItemViewModel.IsSelected))
                        {
                            if (SelectedTreeSideBarItem is null &&
                                       (tsbivm is not null && tsbivm.IsSelected && tsbivm != SelectedTreeSideBarItem))
                            {
                                SelectedTreeSideBarItem = tsbivm;
                            }

                            if (SelectedTreeSideBarItem is not null &&
                                      (tsbivm is not null && tsbivm.IsSelected && tsbivm != SelectedTreeSideBarItem))
                            {
                                SelectedTreeSideBarItem.IsSelected = false;

                                SelectedTreeSideBarItem = tsbivm;
                            }
                            //if (tsbivm.IsSelected)
                            //{
                            //    _selectedTreeSideBarItem = tsbivm;
                            //}
                            //else
                            //{
                            //    _previewSelectedTreeSideBarItem = tsbivm;
                            //}
                        }
                    }
                };
            }

            //bool HasChild(TreeSideBarItemViewModel tsbivm) => (tsbivm is not null) && tsbivm.Children.Any();

            IsLoading = false;
        }
        #endregion

        #region Get All TreeSideBarItemViewModels Method
        internal async Task<TreeSideBarItemViewModel> GetAllTreeSideBarItemViewModelsByMenuItem(MenuItem menuItem)
        {
            List<MenuItem> travelMenuItems = new();
            TreeSideBarItemViewModel virtualParent = new();

            await RecursiveSubMenuItem(menuItem, virtualParent);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem, TreeSideBarItemViewModel paren)
            {
                TreeSideBarItemViewModel child = default;

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

        #region Contain Methods
        private bool AnyEqualsMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isAny = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Title, menuItem.Title) || IsEqualsNameOrTitle(mi.Name, menuItem.Name));

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
