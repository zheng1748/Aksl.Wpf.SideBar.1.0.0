using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Prism.Mvvm;

using Aksl.Infrastructure;
using Aksl.Toolkit.UI;

namespace Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels
{
    public partial class HamburgerMenuSideBarItemViewModel : BindableBase
    {
        #region Popup Properties
        public PopupViewModel ThePopupViewModel { get; set; }

        private bool _isPopupOpen = false;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty<bool>(ref _isPopupOpen, value);
        }

        private PopupSideBarItemViewModel _popupSideBarItemViewModel = default;
        public PopupSideBarItemViewModel SelectedPopupSideBarItem
        {
            get => _popupSideBarItemViewModel;
            set => SetProperty<PopupSideBarItemViewModel>(ref _popupSideBarItemViewModel, value);
        }
        #endregion

        #region Loaded Event
        public void Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.UserControl uc)
            {
                VisualTreeFinder visualTreeFinder = new();
                var listViewItem = visualTreeFinder.FindVisualParent<System.Windows.Controls.ListViewItem>(uc);

                if (listViewItem is not null)
                {
                    listViewItem.MouseEnter += async (sender, e) =>
                    {
                        if (sender is System.Windows.Controls.ListViewItem listViewItem)
                        {
                            listViewItem.Background = new SolidColorBrush(Colors.Honeydew);

                            System.Windows.Point pos = e.GetPosition(uc);
                            HitTestResult result = VisualTreeHelper.HitTest(uc, pos);

                            // Debug.Print($"{listViewItem.GetType()}:MouseEnter");

                            ThePopupViewModel.PlacementTarget = listViewItem;
                            if (ThePopupViewModel.IsOpen)
                            {
                                ThePopupViewModel.IsOpen = false;
                            }
                            ThePopupViewModel.IsOpen = true;

                            await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                        }
                    };
                    listViewItem.MouseLeave += async (sender, e) =>
                    {
                        if (sender is System.Windows.Controls.ListViewItem listViewItem)
                        {
                            listViewItem.Background = new SolidColorBrush(Colors.White);

                            System.Windows.Point point = e.GetPosition(listViewItem);
                            HitTestResult result = VisualTreeHelper.HitTest(listViewItem, point);
                            if (result is not null)
                            {
                                // Debug.Print($"{listViewItem.GetType()}:MouseLeave:{result.VisualHit}");
                            }

                            var childsInListViewItem = visualTreeFinder.FindVisualChilds<DependencyObject>(listViewItem);
                            var popup = childsInListViewItem.FirstOrDefault(d => (d is Popup)) as Popup;
                            if (popup is not null)
                            {
                                System.Windows.Point popupChildPoint = e.GetPosition(popup.Child);

                                bool isMouseInPopup = IsMouseInPopup(popupChildPoint, popup);
                                if (!isMouseInPopup)
                                {
                                    ThePopupViewModel.PlacementTarget = null;
                                    ThePopupViewModel.IsOpen = false;
                                }
                                //Debug.Print($"{isMouseInPopup}:MouseLeave");
                            }

                            await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                        }

                        bool IsMouseInPopup(Point mousePoint, Popup popup)
                        {
                            if (popup.Child is FrameworkElement child)
                            {
                                Rect bounds = new Rect(0, 0, child.ActualWidth, child.ActualHeight);
                                return bounds.Contains(mousePoint);
                            }
                            return false;
                        }
                    };
                }
            }
        }
        #endregion

        #region ExecuteMouseEnter Event
        //public void ExecuteMouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (sender is System.Windows.Controls.UserControl uc)
        //    {
        //        System.Windows.Point pos = e.GetPosition(uc);
        //        HitTestResult result = VisualTreeHelper.HitTest(uc, pos);

        //        VisualTreeFinder visualTreeFinder = new();

        //        var listViewItem = visualTreeFinder.FindVisualParent<System.Windows.Controls.ListViewItem>(uc);

        //        Debug.Print($"{uc.GetType()}:MouseEnter");

        //        PopupViewModel.PlacementTarget = listViewItem;
        //        //if (PopupViewModel.IsOpen)
        //        //{
        //        //    PopupViewModel.IsOpen = false;
        //        //}
        //        PopupViewModel.IsOpen = !PopupViewModel.IsOpen;
        //    }
        //}
        #endregion

        #region ExecuteMouseLeave Event
        //public void ExecuteMouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (sender is System.Windows.Controls.UserControl uc)
        //    {
        //        Debug.Print($"{uc.GetType()}:MouseLeave");

        //        PopupViewModel.PlacementTarget = null;
        //        PopupViewModel.IsOpen = false;
        //    }
        //}
        #endregion

        #region Create PopupSideBarItem ViewModels Method
        internal async Task CreatePopupSideBarItemModelsAsync()
        {
            ObservableCollection<PopupSideBarItemViewModel> allLeafPopupSideBarItems = new();
            IEnumerable<Infrastructure.MenuItem> subMenuItems = default;

            List<Infrastructure.MenuItem> allLeafMenuItems = new();

            if (!string.IsNullOrEmpty(_menuItem.NavigationName))
            {
                var parentMenuItem = await _menuService.GetMenuAsync(_menuItem.NavigationName);
                subMenuItems = parentMenuItem.SubMenus;
            }

            if (string.IsNullOrEmpty(_menuItem.NavigationName) && HasSubMenu(_menuItem) && IsExistsViewInSubMenu(_menuItem))
            {
                subMenuItems = _menuItem.SubMenus.Where(sm => !string.IsNullOrEmpty(sm.ViewName)).ToList();
            }

            bool HasSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any();

            bool IsExistsViewInSubMenu(MenuItem mi) => (mi is not null) && mi.SubMenus.Any(sm => !string.IsNullOrEmpty(sm.ViewName));

            if (subMenuItems is not null && subMenuItems.Any())
            {
                foreach (var smi in subMenuItems)
                {
                    List<MenuItem> travelMenuItems = new();
                    var allLeafPopupSideBarItemViewModels = await GetAllLeafPopupSideBarItemViewModels(smi, travelMenuItems);
                    allLeafPopupSideBarItems.AddRange(allLeafPopupSideBarItemViewModels);
                }

                var allDistinctLeafPopupSideBarItems = allLeafPopupSideBarItems.DistinctBy(item => (item.Name, item.Title));
                allLeafPopupSideBarItems = new ObservableCollection<PopupSideBarItemViewModel>(allDistinctLeafPopupSideBarItems);

                ThePopupViewModel.AllLeafPopupSideBarItems = allLeafPopupSideBarItems;
                ThePopupViewModel.AddPropertyChanged();
                AddPopupPropertyChanged();

                void AddPopupPropertyChanged()
                {
                    ThePopupViewModel.PropertyChanged += (sender, e) =>
                    {
                        if (sender is PopupViewModel pvm)
                        {
                            if (e.PropertyName == nameof(PopupViewModel.IsOpen))
                            {
                                IsPopupOpen = pvm.IsOpen;
                            }

                            if (e.PropertyName == nameof(PopupViewModel.SelectedPopupSideBarItem))
                            {
                                SelectedPopupSideBarItem = pvm.SelectedPopupSideBarItem;
                            }
                        }
                    };
                }
            }
        }
        #endregion

        #region Get All Leaf PopupSideBarItemViewModels Method
        internal async Task<IEnumerable<PopupSideBarItemViewModel>> GetAllLeafPopupSideBarItemViewModels(MenuItem menuItem, IList<MenuItem> travelMenuItems)
        {
            List<PopupSideBarItemViewModel> leafHamburgerMenuSideBarItemViewModels = new();

            await RecursiveSubMenuItem(menuItem);

            async Task RecursiveSubMenuItem(MenuItem currentMenuItem)
            {
                var isAddOnLeaf = IsLeaf(currentMenuItem) && (!HasNavigationName(currentMenuItem) || (HasNavigationName(currentMenuItem) && !IsNextNavigation(currentMenuItem)));
                var isAddOnNotLeaf = !IsLeaf(currentMenuItem) && !IsNexOnNotLeaf(currentMenuItem);
                if (!AnyEqualsMenuItems(travelMenuItems, currentMenuItem) && HasTitle(currentMenuItem) && (isAddOnLeaf || isAddOnNotLeaf))
                {
                    travelMenuItems.Add(currentMenuItem);
                    leafHamburgerMenuSideBarItemViewModels.Add(new(currentMenuItem, null));
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

            return leafHamburgerMenuSideBarItemViewModels;
        }
        #endregion

        #region Contain Methods
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
