using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

namespace Aksl.Modules.HamburgerMenuSideBarTab.ViewModels
{
    public class TabViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        #endregion

        #region Constructors
        public TabViewModel()
        {
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            ActiveTabItems = new();
            //AddCollectionChangedEventHandler();

            StoreTabItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<TabItemViewModel> ActiveTabItems { get; }

        private TabItemViewModel _selectedTabItem;
        public TabItemViewModel SelectedTabItem
        {
            get => _selectedTabItem;
            set
            {
                var previewSelectedHamburgerMenuItem = _selectedTabItem;

                if (SetProperty(ref _selectedTabItem, value))
                {
                    if (previewSelectedHamburgerMenuItem is not null && previewSelectedHamburgerMenuItem.IsSelected)
                    {
                        previewSelectedHamburgerMenuItem.IsSelected = false;
                    }

                    if (_selectedTabItem is not null && !_selectedTabItem.IsSelected)
                    {
                        _selectedTabItem.IsSelected = true;
                    }
                }
            }
        }

        public List<TabItemViewModel> StoreTabItems { get; }
        #endregion

        #region Methods
        //public void AddCollectionChangedEventHandler()
        //{
        //    ActiveTabItems.CollectionChanged += (sender, e) =>
        //    {
        //        if (e.NewItems is not null && e.NewItems.Count != 0)
        //        {
        //            foreach (TabItemViewModel tabItemViewModel in e.NewItems)
        //            {
        //                tabItemViewModel.RequestClose += this.OnTabItemRequestClose;
        //            }
        //        }

        //        if (e.OldItems is not null && e.OldItems.Count != 0)
        //        {
        //            foreach (TabItemViewModel tabItemViewModel in e.OldItems)
        //            {
        //                tabItemViewModel.RequestClose -= this.OnTabItemRequestClose;
        //            }
        //        }
        //    };
        //}

        private void OnTabItemRequestClose(object sender, EventArgs e)
        {
            if (sender is TabItemViewModel tabItemViewModel)
            {
                Remove(tabItemViewModel);
            }
        }

        public void Add(TabInformation tabInformation)
        {
            TabItemViewModel newTabItemViewModel = new(tabInformation);

            if (tabInformation.ViewElement is not null)
            {
                newTabItemViewModel.ViewElement = tabInformation.ViewElement;
            }

            AddCore(newTabItemViewModel);

            //TabItemViewModel newTabItemViewModel=new (menuItem);

            //TabItems.Add(newTabItemViewModel);

            //SetActiveTabItem(newTabItemViewModel);

            //RaisePropertyChanged(nameof(TabItems));
        }

        private void AddCore(TabItemViewModel newTabItemViewModel)
        {
            if (!IsExistsActivTabItems(newTabItemViewModel.Name, newTabItemViewModel.Title))
            {
                ActiveTabItems.Add(newTabItemViewModel);

                newTabItemViewModel.RequestClose += this.OnTabItemRequestClose;
            }

            StoreTabItem(newTabItemViewModel);

            SetActiveTabItem(newTabItemViewModel);

            RaisePropertyChanged(nameof(ActiveTabItems));
        }

        private void StoreTabItem(TabItemViewModel newTabItemViewModel)
        {
            if (!IsExistsStoreTabItems(newTabItemViewModel.Name, newTabItemViewModel.Title))
            {
                StoreTabItems.Add(newTabItemViewModel);
            }
        }

        public void Remove(TabItemViewModel tabItemViewModel)
        {
            if (tabItemViewModel is not null)
            {
                if (IsExistsActivTabItems(tabItemViewModel.Name, tabItemViewModel.Title))
                {
                    ActiveTabItems.Remove(tabItemViewModel);
                    tabItemViewModel.RequestClose -= this.OnTabItemRequestClose;
                }

                if (!ActiveTabItems.Any())
                {
                    SelectedTabItem = null;

                    _eventAggregator.GetEvent<OnSelectedTabItemEmptyEvent>().Publish(new());
                }

                RaisePropertyChanged(nameof(ActiveTabItems));
            }
        }

        public void SetFirstActiveTabItem()
        {
            if (StoreTabItems.Any())
            {
                var storeTabItemViewMode = StoreTabItems.First();
                SelectedTabItem = storeTabItemViewMode;
            }
        }

        private void SetActiveTabItem(TabItemViewModel tabItemViewModel)
        {
            if (tabItemViewModel is not null && !IsEqualsTabItemViewModel(tabItemViewModel, SelectedTabItem))
            {
                SelectedTabItem = tabItemViewModel;
            }

            //if (tabItemViewModel is not null)
            //{
            //    ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.TabItems);
            //    if (collectionView is not null)
            //    {
            //        collectionView.MoveCurrentTo(tabItemViewModel);
            //    }

            //    collectionView.Refresh();
            //}
        }

        public void SetTabItem(TabInformation tabInformation)
        {
            var activeTabItem = GetActiveTabItemViewModel(tabInformation);
            if (activeTabItem is not null)
            {
                SetActiveTabItem(activeTabItem);
            }
            else
            {
                var storeTabItemViewModel = GetStoreTabItemViewModel(tabInformation);
                if (storeTabItemViewModel is not null)
                {
                    AddCore(storeTabItemViewModel);
                }
            }
        }

        public void RetsetTabItem(TabInformation tabInformation)
        {
            var activeTabItem = GetActiveTabItemViewModel(tabInformation);
            if (activeTabItem is not null)
            {
                activeTabItem.ViewElement = null;

                if (tabInformation.ViewElement is not null)
                {
                    activeTabItem.ViewElement = tabInformation.ViewElement;
                }

                SetActiveTabItem(activeTabItem);
            }
            else
            {
                var storeTabItemViewModel = GetStoreTabItemViewModel(tabInformation);
                if (storeTabItemViewModel is not null)
                {
                    storeTabItemViewModel.ViewElement = null;

                    if (tabInformation.ViewElement is not null)
                    {
                        storeTabItemViewModel.ViewElement = tabInformation.ViewElement;
                    }

                    ActiveTabItems.Add(storeTabItemViewModel);
                    storeTabItemViewModel.RequestClose += this.OnTabItemRequestClose;

                    SetActiveTabItem(storeTabItemViewModel);
                }
            }
        }

        private TabItemViewModel GetActiveTabItemViewModel(TabInformation tabInformation)
        {
            var activeTabItemViewModel = ActiveTabItems.FirstOrDefault(ti => IsEqualsNameOrTitle(ti.Name, tabInformation.Name) || IsEqualsNameOrTitle(ti.Title, tabInformation.Title));

            return activeTabItemViewModel;
        }

        public TabItemViewModel GetStoreTabItemViewModel(TabInformation tabInformation)
        {
            var storeTabItemViewModel = StoreTabItems.FirstOrDefault(ti => IsEqualsNameOrTitle(ti.Name, tabInformation.Name) || IsEqualsNameOrTitle(ti.Title, tabInformation.Title));

            return storeTabItemViewModel;
        }

        public System.Windows.DependencyObject GetStoreViewElement(Type viewType)
        {
            var storeTabItemViewModel = StoreTabItems.FirstOrDefault(ti => ti.ViewElementType == viewType);

            return storeTabItemViewModel?.ViewElement;
        }

        //public object GetActiveViewElementType(Type viewType)
        //{
        //    var view = ActiveTabItems.FirstOrDefault(ti => ti.ViewElementType == viewType);

        //    return view;
        //}

        public bool IsActiveTabItem(TabInformation tabInformation)
        {
            var _isExists = ActiveTabItems.Any(ti => ti.IsSelected && (IsEqualsNameOrTitle(ti.Name, tabInformation.Name) || IsEqualsNameOrTitle(ti.Title, tabInformation.Title)));

            var activeTabItemViewModel = ActiveTabItems.FirstOrDefault(ti => ti.IsSelected);
            var isExists = IsEqualsNameOrTitle(activeTabItemViewModel?.Name, tabInformation.Name) || IsEqualsNameOrTitle(activeTabItemViewModel?.Title, tabInformation.Title);

            return isExists;
        }
        #endregion

        #region Contain Methods
        private bool IsExistsActivTabItems(string name, string title)
        {
            var isExists = ActiveTabItems.Any(ti => IsEqualsNameOrTitle(ti.Name, name) || IsEqualsNameOrTitle(ti.Title, title));

            return isExists;
        }

        private bool IsExistsStoreTabItems(string name, string title)
        {
            var isExists = StoreTabItems.Any(ti => IsEqualsNameOrTitle(ti.Name, name) || IsEqualsNameOrTitle(ti.Title, title));

            return isExists;
        }

        private bool IsEqualsTabItemViewModel(TabItemViewModel tabItemViewModel, TabItemViewModel otherTabItemViewModel)
        {
            var isEquals = (IsEqualsNameOrTitle(tabItemViewModel?.Name, otherTabItemViewModel?.Name) ||
                            IsEqualsNameOrTitle(tabItemViewModel?.Title, otherTabItemViewModel?.Title));

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
