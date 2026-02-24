using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;

namespace Aksl.Modules.MenuSub.ViewModels
{
     public class HierarchicalMenusViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        private MenuItem _rootMenuItem;
        private MenuItem _parentMenuItem;
        private HierarchicalMenuItemViewModel _selectedMenuItem;
        #endregion

        #region Constructors
        public HierarchicalMenusViewModel(IEventAggregator eventAggregator,IMenuService menuService, MenuItem rootMenuItem)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            _rootMenuItem = rootMenuItem;

            //TopHierarchicalMenuItems = new();
            //TopLeafHierarchicalMenuItems = new();
            //AllHierarchicalSubMenuItems = new();
            //AllLeafHierarchicalSubMenuItems = new();

            AllHierarchicalMenuItems = new();

            RegisterOnTopMenuSelectedEvent();
        }
        #endregion

        #region Properties
        //public ObservableCollection<HierarchicalMenuItemViewModel> TopHierarchicalMenuItems { get; }
        //public ObservableCollection<HierarchicalMenuItemViewModel> TopLeafHierarchicalMenuItems { get; }
        //public ObservableCollection<HierarchicalMenuItemViewModel> AllHierarchicalSubMenuItems { get; }
        //public ObservableCollection<HierarchicalMenuItemViewModel> AllLeafHierarchicalSubMenuItems { get; private set; }
        public ObservableCollection<HierarchicalMenuItemViewModel> AllHierarchicalMenuItems { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Register TopMenu Selected Event
        private void RegisterOnTopMenuSelectedEvent()
        {
            _eventAggregator.GetEvent<OnTopMenuSubSelectedEvent>().Subscribe((tmsse) =>
            {
                ClearTopSelectedHierarchicalMenuItemViewModels();

                var nowTopHierarchicalMenuItem = OnlyFindTopHierarchicalMenuItemViewModelByMenuItem(tmsse.SelectedMenuItem);

                if (nowTopHierarchicalMenuItem.Top is not null && nowTopHierarchicalMenuItem.Sub is not null && nowTopHierarchicalMenuItem.Top == nowTopHierarchicalMenuItem.Sub)
                {
                    if (_selectedMenuItem is not null && _selectedMenuItem.IsSubmenuItem)
                    {
                        nowTopHierarchicalMenuItem.Top.IsTopLevelSelected = true;
                    }

                    if (_selectedMenuItem is not null && _selectedMenuItem.IsTopLevelItem && _selectedMenuItem == nowTopHierarchicalMenuItem.Top)
                    {
                        nowTopHierarchicalMenuItem.Top.IsTopLevelSelected = true;
                    }
                }

                if (nowTopHierarchicalMenuItem.Top is not null && nowTopHierarchicalMenuItem.Sub is not null && nowTopHierarchicalMenuItem.Top != nowTopHierarchicalMenuItem.Sub)
                {
                    if (_selectedMenuItem is not null && _selectedMenuItem.IsSubmenuItem && _selectedMenuItem == nowTopHierarchicalMenuItem.Sub)
                    {
                        nowTopHierarchicalMenuItem.Top.IsTopLevelSelected = true;
                    }
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Find Top HierarchicalMenuItemViewModel Methods
        private (HierarchicalMenuItemViewModel Top, HierarchicalMenuItemViewModel Sub) OnlyFindTopHierarchicalMenuItemViewModelByMenuItem(MenuItem menuItem)
        {
            HierarchicalMenuItemViewModel topHierarchicalMenuItemViewModel = null;
            HierarchicalMenuItemViewModel findHierarchicalMenuItemViewModel = null;

            int i = 0;
            for (i = 0; i < AllHierarchicalMenuItems.Count; i++)
            {
                if (findHierarchicalMenuItemViewModel is null)
                {
                    RecursiveSubMenuItemViewModel(AllHierarchicalMenuItems[i]);
                }
                else
                {
                    topHierarchicalMenuItemViewModel = AllHierarchicalMenuItems[i - 1];
                    break;
                }
            }

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel parent)
            {
                if (IsEqualsNameOrTitle(parent.Name, menuItem.Name) || IsEqualsNameOrTitle(parent.Title, menuItem.Title))
                {
                    findHierarchicalMenuItemViewModel = parent;
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

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            if (findHierarchicalMenuItemViewModel is not null && i == AllHierarchicalMenuItems.Count)
            {
                topHierarchicalMenuItemViewModel = AllHierarchicalMenuItems[i - 1];
            }

            return (Top: topHierarchicalMenuItemViewModel, Sub: findHierarchicalMenuItemViewModel);
        }
        #endregion

        #region Create HierarchicalMenuItem ViewModels Method
        internal async Task CreateHierarchicalMenuItemViewModelsAsync()
        {
            IsLoading = true;

            var parentMenuItem = await _menuService.GetMenuAsync(_rootMenuItem.NavigationName);

            _parentMenuItem = parentMenuItem;

            var subMenuItems = _parentMenuItem.SubMenus;
            foreach (var smi in subMenuItems)
            {
                //HierarchicalMenuItemViewModel topHierarchicalMenuItemViewModel = new(_eventAggregator, smi);
                //TopHierarchicalMenuItems.Add(topHierarchicalMenuItemViewModel);
                //var topLeafHierarchicalMenuItemViewModels = GetTopLeafHierarchicalMenuItemViewModels(topHierarchicalMenuItemViewModel);
                //TopLeafHierarchicalMenuItems.AddRange(topLeafHierarchicalMenuItemViewModels);

                //HierarchicalMenuItemViewModel parent = new(_eventAggregator, smi);
                //AllHierarchicalSubMenuItems.Add(parent);
                //List<MenuItem> allTravelMenuItems = new();
                //await GetAllHierarchicalSubMenuItemViewModelsAsync(smi, allTravelMenuItems, parent);

                List<MenuItem> allTravelMenuItems = new();
                var topViewModel = await GetHierarchicalMenuItemViewModelsByMenuItem(smi, allTravelMenuItems);
                AllHierarchicalMenuItems.Add(topViewModel);
            }

            //var allLeafHierarchicalSubMenuItemViewModels = GetAllLeafHierarchicalSubMenuItemViewModels();
            //AllLeafHierarchicalSubMenuItems = new(allLeafHierarchicalSubMenuItemViewModels.AllLeaf);

            SetWorkspaceViewEventNameAndPropertyChanged();

            void SetWorkspaceViewEventNameAndPropertyChanged()
            {
                foreach (var tmi in AllHierarchicalMenuItems)
                {
                    RecursiveSubMenuItem(tmi);
                }

                void RecursiveSubMenuItem(HierarchicalMenuItemViewModel hierarchicalMenuItemViewModel)
                {
                    AddPropertyChanged(hierarchicalMenuItemViewModel);

                    if (hierarchicalMenuItemViewModel.IsLeaf)
                    {
                        hierarchicalMenuItemViewModel.WorkspaceViewEventName = _rootMenuItem.WorkspaceViewEventName;
                    }

                    if (HasChild(hierarchicalMenuItemViewModel))
                    {
                        foreach (var smi in hierarchicalMenuItemViewModel.Children)
                        {
                            RecursiveSubMenuItem(smi);
                        }
                    }
                }
            }

            void AddPropertyChanged(HierarchicalMenuItemViewModel curentHierarchicalMenuItemViewModel)
            {
                curentHierarchicalMenuItemViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is HierarchicalMenuItemViewModel hmvm)
                    {
                        if (e.PropertyName == nameof(HierarchicalMenuItemViewModel.IsSelected))
                        {
                            ClearTopSelectedHierarchicalMenuItemViewModels();

                            var selectedMenuItemCount = GetSelectedHierarchicalMenuItemViewModels().Count();
                            if (_selectedMenuItem is null && hmvm.IsSelected)
                            {
                                _selectedMenuItem = hmvm;
                                _selectedMenuItem.IsSelected = true;
                            }
                            else if ((_selectedMenuItem is not null) && !IsEqualsNameOrTitle(_selectedMenuItem?.Name, hmvm?.Name))
                            {
                                _selectedMenuItem.IsSelected = false;
                                _selectedMenuItem = hmvm;
                            }
                            selectedMenuItemCount = GetSelectedHierarchicalMenuItemViewModels().Count();
                        }
                    }
                };
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            IsLoading = false;
        }
        #endregion

        #region Get All Hierarchical HierarchicalMenuItemViewModels Method
        internal async Task<HierarchicalMenuItemViewModel> GetHierarchicalMenuItemViewModelsByMenuItem(MenuItem menuItem, IList<MenuItem> travelMenuItems)
        {
            HierarchicalMenuItemViewModel virtualParent = new();

            await RecursiveSubMenuItem(menuItem, virtualParent);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem, HierarchicalMenuItemViewModel paren)
            {
                HierarchicalMenuItemViewModel child = default;

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

        #region Get Top Leaf HierarchicalMenuItemViewModel Method
        internal IEnumerable<HierarchicalMenuItemViewModel> GetTopLeafHierarchicalMenuItemViewModels(HierarchicalMenuItemViewModel topHieMenuItemViewModel)
        {
            List<HierarchicalMenuItemViewModel> topLeafHierarchicalMenuItemViewModels = new();

            RecursiveSubMenuItemViewModel(topHieMenuItemViewModel);

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel currenyHieMenuItemViewModel)
            {
                if (!AnyEqualsHierarchicalMenuItemViewModels(topLeafHierarchicalMenuItemViewModels, currenyHieMenuItemViewModel) && currenyHieMenuItemViewModel.IsLeaf && currenyHieMenuItemViewModel.HasTitle)
                {
                    topLeafHierarchicalMenuItemViewModels.Add(currenyHieMenuItemViewModel);
                }

                if (HasChild(currenyHieMenuItemViewModel))
                {
                    foreach (var children in currenyHieMenuItemViewModel.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return topLeafHierarchicalMenuItemViewModels;
        }
        #endregion

        #region Get All Hierarchical SubMenuItemViewModels Methods
        private async Task GetAllHierarchicalSubMenuItemViewModelsAsync(MenuItem menuItem, IList<MenuItem> travelMenuItems, HierarchicalMenuItemViewModel currentHieMenuItemViewModel)
        {
            #region Method

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem))
                {
                    travelMenuItems.Add(currentMenuItem);
                }

                var matchResult = FindMatchHierarchicalMenuItemViewModelByMenuItem(currentHieMenuItemViewModel, currentMenuItem);
                Debug.Assert(matchResult.IsTrue);
                if (HasNavigationName(currentMenuItem) && IsNextNavigation(currentMenuItem) && IsLeaf(currentMenuItem) && matchResult.FindHierarchicalMenuItemViewModel.IsLeaf)
                {
                    currentMenuItem = await _menuService.GetMenuAsync(currentMenuItem.NavigationName);

                    if (HasSubMenu(currentMenuItem))
                    {
                        var parent = matchResult.FindHierarchicalMenuItemViewModel;

                        foreach (var smi in currentMenuItem.SubMenus)
                        {
                            HierarchicalMenuItemViewModel menuItemViewModel = new(_eventAggregator, smi, parent);
                            parent.Children.Add(menuItemViewModel);
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

        private (HierarchicalMenuItemViewModel FindHierarchicalMenuItemViewModel, bool IsTrue) FindMatchHierarchicalMenuItemViewModelByMenuItem(HierarchicalMenuItemViewModel hieMenuItemViewModel, MenuItem menuItem)
        {
            var findViewModel = FindHierarchicalMenuItemViewModelByMenuItem(hieMenuItemViewModel, menuItem);

            return (FindHierarchicalMenuItemViewModel: findViewModel, IsTrue: (findViewModel is not null));
        }

        internal HierarchicalMenuItemViewModel FindHierarchicalMenuItemViewModelByMenuItem(HierarchicalMenuItemViewModel hierarchicalMenuItemViewModel, MenuItem menuItem)
        {
            HierarchicalMenuItemViewModel findHierarchicalMenuItemViewModel = null;

            RecursiveSubMenuItemViewModel(hierarchicalMenuItemViewModel);

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel parent)
            {
                //if (IsEqualsHierarchicalMenuItemViewModel(parent, nameOrTitle))
                if (IsEqualsNameOrTitle(parent.Name, menuItem.Name) || IsEqualsNameOrTitle(parent.Title, menuItem.Title))
                {
                    findHierarchicalMenuItemViewModel = parent;
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

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return findHierarchicalMenuItemViewModel;
        }
        #endregion

        #region Get All LeafHierarchical SubMenuItemViewModels Methods
        internal (IEnumerable<HierarchicalMenuItemViewModel> AllLeaf, Dictionary<HierarchicalMenuItemViewModel, IEnumerable<HierarchicalMenuItemViewModel>> Lookup) GetAllLeafHierarchicalSubMenuItemViewModels()
        {
            Dictionary<HierarchicalMenuItemViewModel, IEnumerable<HierarchicalMenuItemViewModel>> rootLeafHierarchicalSubMenuItemLookup = new();
            List<HierarchicalMenuItemViewModel> allLeafHierarchicalMenuItemViewModels = new();

            List<HierarchicalMenuItemViewModel> leafHierarchicalMenuItemViewModels = new();

            foreach (var hmivm in AllHierarchicalMenuItems)
            {
                RecursiveSubMenuItemViewModel(hmivm);

                rootLeafHierarchicalSubMenuItemLookup.Add(hmivm, leafHierarchicalMenuItemViewModels);

                leafHierarchicalMenuItemViewModels = new();
            }

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel currenyHieMenuItemViewModel)
            {
                if (!AnyEqualsHierarchicalMenuItemViewModels(leafHierarchicalMenuItemViewModels, currenyHieMenuItemViewModel) && currenyHieMenuItemViewModel.IsLeaf && currenyHieMenuItemViewModel.HasTitle)
                {
                    leafHierarchicalMenuItemViewModels.Add(currenyHieMenuItemViewModel);
                    allLeafHierarchicalMenuItemViewModels.Add(currenyHieMenuItemViewModel);
                }

                if (HasChild(currenyHieMenuItemViewModel))
                {
                    foreach (var children in currenyHieMenuItemViewModel.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return (AllLeaf: allLeafHierarchicalMenuItemViewModels, Lookup: rootLeafHierarchicalSubMenuItemLookup);
        }
        #endregion

        #region Clear TopSelected HierarchicalMenuItemViewModels Method
        private void ClearTopSelectedHierarchicalMenuItemViewModels()
        {
            var topLevelMenuItemViewModels = GetTopSelectedHierarchicalMenuItemViewModels();
            foreach (var tmivm in topLevelMenuItemViewModels)
            {
                if (tmivm is not null && tmivm.IsTopLevelSelected)
                {
                    tmivm.IsTopLevelSelected = false;
                }
            }
        }
        #endregion

        #region Get TopSelected HierarchicalMenuItemViewModels Method
        private List<HierarchicalMenuItemViewModel> GetTopSelectedHierarchicalMenuItemViewModels()
        {
            List<HierarchicalMenuItemViewModel> hieMenuItemViewModels = new();

            foreach (var hmivm in AllHierarchicalMenuItems)
            {
                RecursiveSubMenuItemViewModel(hmivm);
            }

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel parent)
            {
                if (parent.IsTopLevelSelected)
                {
                    hieMenuItemViewModels.Add(parent);
                }

                if (HasChild(parent))
                {
                    foreach (var children in parent.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return hieMenuItemViewModels;
        }
        #endregion

        #region Get Selected HierarchicalMenuItemViewModels Method
        private List<HierarchicalMenuItemViewModel> GetSelectedHierarchicalMenuItemViewModels()
        {
            List<HierarchicalMenuItemViewModel> findHierarchicalMenuItemViewModels = new();

            foreach (var hmivm in AllHierarchicalMenuItems)
            {
                RecursiveSubMenuItemViewModel(hmivm);
            }

            void RecursiveSubMenuItemViewModel(HierarchicalMenuItemViewModel parent)
            {
                if (parent.IsSelected)
                {
                    findHierarchicalMenuItemViewModels.Add(parent);
                }

                if (HasChild(parent))
                {
                    foreach (var children in parent.Children)
                    {
                        RecursiveSubMenuItemViewModel(children);
                    }
                }
            }

            bool HasChild(HierarchicalMenuItemViewModel hmivm) => (hmivm is not null) && hmivm.Children.Any();

            return findHierarchicalMenuItemViewModels;
        }
        #endregion

        #region Contain Methods
        private bool AnyEqualsHierarchicalMenuItemViewModels(IEnumerable<HierarchicalMenuItemViewModel> hierarchicalMenuItemViewModels, HierarchicalMenuItemViewModel hierarchicalMenuItemViewModel)
        {
            var isEquals = hierarchicalMenuItemViewModels.Any(hmivm => IsEqualsNameOrTitle(hmivm.Name, hierarchicalMenuItemViewModel.Name) || IsEqualsNameOrTitle(hmivm.Title, hierarchicalMenuItemViewModel.Title));

            return isEquals;
        }

        private bool AnyEqualsMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem)
        {
            var isEquals = menuItems.Any(mi => IsEqualsNameOrTitle(mi.Name, menuItem.Name) || IsEqualsNameOrTitle(mi.Title, menuItem.Title));

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
