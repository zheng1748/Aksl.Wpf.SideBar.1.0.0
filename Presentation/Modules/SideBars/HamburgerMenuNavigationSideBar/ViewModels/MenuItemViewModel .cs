using System;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;

using Aksl.Toolkit.Controls;
using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class MenuItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        //public MenuItemViewModel(MenuItem menuItem)
        //{
        //    _menuItem = menuItem;
        //}

        public MenuItemViewModel(IEventAggregator eventAggregator, int groupIndex, int index, MenuItem menuItem)
        {
            _eventAggregator = eventAggregator;
            GroupIndex = groupIndex;
            Index = index;
            _menuItem = menuItem;
        }
        #endregion

        #region Properties
        public MenuItem MenuItem => _menuItem;
        public string WorkspaceViewEventName { get; set; }
        public int GroupIndex { get; }
        public int Index { get; }
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

                    //if (IsLeaf && _isSelected)
                    if (isSelectedOnLeaf && _isSelected)
                    {
                        //_eventAggregator.GetEvent<OnBuildHamburgerMenuNavigationSideBarWorkspaceViewEvent>().Publish(new() { CurrentMenuItem = _menuItem });
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
    }
}
