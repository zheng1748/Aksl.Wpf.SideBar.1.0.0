using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Unity;
using Unity;

using Aksl.Dialogs.Services;
using Aksl.Toolkit.Controls;
using Aksl.Toolkit.UI;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class NoGroupedMenuItemViewModel :   BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator; 
        private readonly IDialogViewService _dialogViewService;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public NoGroupedMenuItemViewModel(int index, MenuItem menuItem)
        {
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            Index = index;
            _menuItem = menuItem;
        }
        #endregion

        #region Properties
        public MenuItem MenuItem => _menuItem;
        public int Index { get; }
        public string WorkspaceViewEventName { get; set; }
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;
        public bool IsLeaf => _menuItem.SubMenus.Count <= 0;
        private bool IsNextNavigation => _menuItem.IsNextNavigation;
        private bool HasNavigationName => !string.IsNullOrEmpty(_menuItem.NavigationName);
        private bool IsNexOnNotLeaf => _menuItem.IsNexOnNotLeaf;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    var isSelectedOnLeaf = IsLeaf && (!HasNavigationName || (HasNavigationName && !IsNextNavigation));
                    var isSelectedOnNotLeaf = !IsLeaf && !IsNexOnNotLeaf;

                    if (isSelectedOnLeaf && _isSelected)
                    {
                        var buildHWorkspaceViewEvent = _eventAggregator.GetEvent(WorkspaceViewEventName) as OnBuildWorkspaceViewEventbase;
                        buildHWorkspaceViewEvent.Publish(new() { CurrentMenuItem = _menuItem });
                    }

                    if (isSelectedOnNotLeaf && _isSelected)
                    {
                        var buildHWorkspaceViewEvent = _eventAggregator.GetEvent(WorkspaceViewEventName) as OnBuildWorkspaceViewEventbase;
                        buildHWorkspaceViewEvent.Publish(new() { CurrentMenuItem = _menuItem });
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
        public void ExecuteLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.UserControl uc)
            {
                try
                {
                    VisualTreeFinder visualTreeFinder = new();
                    var grid = visualTreeFinder.FindVisualChild<System.Windows.Controls.Grid>(uc);

                    grid.PreviewMouseLeftButtonDown+= (sender, e) =>
                    {
                        IsSelected = true;
                    };

                }
                catch (Exception ex)
                {
                    _dialogViewService.AlertAsync(message: $"Loaded Error: \"{ex.Message}\"", title: "Error").Await();
                }
            }
        }
        #endregion

        #region MouseLeftButtonDown Event
        public void ExecuteMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Grid grid)
            {
                VisualTreeFinder visualTreeFinder = new();
                
                IsSelected = true;

                e.Handled = true;
            }
        }
        #endregion

        #region MouseMove Event
        public void ExecuteMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.Grid grid)
            {
               // grid.Background=new SolidColorBrush( System.Windows.Media.Colors.Blue);

                e.Handled = true;
            }
        }
        #endregion
    }
}
