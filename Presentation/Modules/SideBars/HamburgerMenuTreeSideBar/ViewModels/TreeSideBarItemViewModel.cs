using System;
using System.Collections.ObjectModel;
using System.Linq;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;
using Aksl.Toolkit.Controls;

namespace Aksl.Modules.HamburgerMenuTreeSideBar.ViewModels
{
    public class TreeSideBarItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        protected readonly TreeSideBarItemViewModel _parent;
        protected ObservableCollection<TreeSideBarItemViewModel> _children;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public TreeSideBarItemViewModel()
        {
            _menuItem = null;
            Parent = null;

            _children = new();
        }

        public TreeSideBarItemViewModel(MenuItem menuItem)
        {
            _menuItem = menuItem;
            Parent = null;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            _children = new();
        }

        public TreeSideBarItemViewModel(MenuItem menuItem, TreeSideBarItemViewModel parent)
        {
            _menuItem = menuItem;
            Parent = parent;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            Parent?.Children.Add(this);

            _children = new();
        }

        public TreeSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
        }

        public TreeSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, TreeSideBarItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new((from child in _menuItem.SubMenus
                             select new TreeSideBarItemViewModel(eventAggregator, child, this)).ToList<TreeSideBarItemViewModel>());
        }
        #endregion

        #region Properties 
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;
        public string WorkspaceViewEventName { get; set; }
        public int Level => _menuItem.Level;
        public TreeSideBarItemViewModel Parent { get; set; }
        public ObservableCollection<TreeSideBarItemViewModel> Children => _children;
        public bool IsLeaf => (_children is not null) && _children.Count <= 0;
        public bool IsTopLevelItem => (Parent is null) && IsLeaf;
        public bool IsTopLevelHeader => (Parent is null) && !IsLeaf;
        public bool IsSubmenuItem => (Parent is not null) && IsLeaf;
        public bool IsSubmenuHeader => (Parent is not null) && !IsLeaf;

        protected bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (IsLeaf && _isSelected)
                    {
                        var buildHWorkspaceViewEvent = _eventAggregator.GetEvent(WorkspaceViewEventName) as OnBuildWorkspaceViewEventbase;
                        buildHWorkspaceViewEvent.Publish(new() { CurrentMenuItem = _menuItem });
                    }
                }
            }
        }

        protected bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                SetProperty<bool>(ref _isExpanded, value);

                if (_isExpanded && Parent is not null)
                {
                    if (!Parent.IsExpanded)
                    {
                        Parent.IsExpanded = true;
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

        protected bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty<bool>(ref _isEnabled, value))
                {
                    //ForceChildEnabled(this, _isEnabled);
                    foreach (var children in this.Children)
                    {
                        children.IsEnabled = _isEnabled;
                    }
                }
            }
        }
        #endregion
    }
}