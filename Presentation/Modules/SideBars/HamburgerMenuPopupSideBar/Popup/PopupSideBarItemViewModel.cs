using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Toolkit.Controls;
using Aksl.Toolkit.UI;

namespace Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels
{
    public class PopupSideBarItemViewModel : BindableBase
    {
        #region Members 
        private readonly IUnityContainer _container;
        protected readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        protected readonly PopupSideBarItemViewModel _parent;
        protected ObservableCollection<PopupSideBarItemViewModel> _children;
        private readonly Infrastructure.MenuItem _menuItem;
        #endregion

        #region Constructors
        public PopupSideBarItemViewModel(Infrastructure.MenuItem menuItem, PopupSideBarItemViewModel parent)
        {
            _menuItem = menuItem;
            Parent = parent;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            Parent?.Children.Add(this);

            _children = new();
        }
        #endregion

        #region Properties
        public MenuItem MenuItem => _menuItem;
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;
        public string WorkspaceViewEventName { get; set; }
        public int Level => _menuItem.Level;
        public string NavigationNam => _menuItem.NavigationName;
        public bool IsSelectedOnInitialize => _menuItem.IsSelectedOnInitialize;
        public PopupSideBarItemViewModel Parent { get; set; }
        public ObservableCollection<PopupSideBarItemViewModel> Children => _children;
        public bool HasChildren => (_children is not null) && _children.Any();
        public bool HasTitle => !string.IsNullOrEmpty(_menuItem.Title);
        public bool IsLeaf => (_children is not null) && _children.Count <= 0;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (IsLeaf && _isSelected)
                    {
                    }
                }
            }
        }

        public PackIconKind IconKind
        {
            get
            {
                PackIconKind kind = PackIconKind.None;

                _ = Enum.TryParse(_menuItem.IconKind, out kind);

                return kind;
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => SetProperty<bool>(ref _isPaneOpen, value);
        }

        protected bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;

            set => SetProperty<bool>(ref _isEnabled, value);
        }
        #endregion

        #region Loaded Event
        public void Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.UserControl uc)
            {
                VisualTreeFinder visualTreeFinder = new();
                var listViewItem = visualTreeFinder.FindVisualParent<System.Windows.Controls.ListViewItem>(uc);

                if (listViewItem != null)
                {
                    listViewItem.MouseLeave += (sender, e) =>
                    {
                        if (sender is System.Windows.Controls.ListViewItem listViewItem)
                        {
                            listViewItem.Background = new SolidColorBrush(Colors.White);

                            System.Windows.Point point = e.GetPosition(listViewItem);
                            HitTestResult result = VisualTreeHelper.HitTest(listViewItem, point);
                            //if (result is not null)
                            //{
                            //    Debug.Print($"{listViewItem.GetType()}:VisualHit:{result.VisualHit}");
                            //}

                            VisualTreeFinder visualTreeFinder = new();
                            var parentsToListViewItem = visualTreeFinder.FindVisualParents<DependencyObject>(listViewItem);
                            var listView = parentsToListViewItem.FirstOrDefault(d => d is System.Windows.Controls.ListView) as System.Windows.Controls.ListView;
                        
                            var popupRoot = parentsToListViewItem.FirstOrDefault(d => d.GetType().Name == "PopupRoot") as FrameworkElement;

                            if (popupRoot is not null)
                            {
                                var popup = visualTreeFinder.FindLogicalParent<System.Windows.Controls.Primitives.Popup>(popupRoot);
                                if (popup is not null)
                                {
                                    System.Windows.Point popupChildPoint = e.GetPosition(popup.Child);

                                    //bool isMouseInPopup = IsMouseInPopup(popupChildPoint, popup);
                                    //Debug.Print($"{isMouseInPopup}:MouseLeave");
                                    //if (!isMouseInPopup)
                                    //{
                                    //    PopupViewModel popupViewModel = popup.DataContext as PopupViewModel;

                                    //    if (popupViewModel is not null)
                                    //    {
                                    //        popupViewModel.PlacementTarget = null;
                                    //        popupViewModel.IsOpen = false;
                                    //    }
                                    //}

                                    Point listViewPoint = e.GetPosition(listViewItem);
                                    bool isMouseInListView = IsMouseOverInListView(listViewPoint, listView);
                                  //  Debug.Print($"{isMouseInListView}:MouseLeave");
                                    if (!isMouseInListView)
                                    {
                                        //PopupViewModel popupViewModel = popup.DataContext as PopupViewModel;

                                        //if (popupViewModel is not null)
                                        //{
                                        //    popupViewModel.PlacementTarget = null;
                                        //    popupViewModel.IsOpen = false;
                                        //}
                                    }

                                  //  await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
                                }

                                int index= listView.GetIndexUnderCursor();
                                Debug.Print($"{index}:MouseLeave");
                                if (index<=-1)
                                {
                                    var popupViewModel = popup.DataContext as PopupViewModel;

                                    if (popupViewModel is not null)
                                    {
                                        popupViewModel.PlacementTarget = null;
                                        popupViewModel.IsOpen = false;
                                    }
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

                                bool IsMouseOverInListView(Point mousePoint,System.Windows.Controls.ListView target)
                                {
                                    Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
                                    return bounds.Contains(mousePoint);
                                }

                                int IndexUnderCursor()
                                {
                                    int index = -1;
                                    for (int i = 0; i < listView.Items.Count; ++i)
                                    {
                                        System.Windows.Controls.ListViewItem item = listView.ItemContainerGenerator.ContainerFromIndex(i) as System.Windows.Controls.ListViewItem;
                                      
                                        if (IsMouseOver(item))
                                        {
                                            index = i;
                                            break;
                                        }
                                    }
                                    return index;
                                }

                                bool IsMouseOver(FrameworkElement target)
                                {
                                    Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
                                    // Point mousePos = e.GetPosition(element);
                                    Point mousePos = MouseUtilities.GetMousePosition(target);
                                    return bounds.Contains(mousePos);
                                }
                            }
                        }
                    };
                }
            }
        }
        #endregion

    }
}
