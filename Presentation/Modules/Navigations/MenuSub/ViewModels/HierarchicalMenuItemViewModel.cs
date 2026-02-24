using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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

namespace Aksl.Modules.MenuSub.ViewModels
{
    public class HierarchicalMenuItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        protected readonly HierarchicalMenuItemViewModel _parent;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public HierarchicalMenuItemViewModel()
        {
            _menuItem = null;
            Parent = null;

            _children = new();
        }

        public HierarchicalMenuItemViewModel(MenuItem menuItem)
        {
            _menuItem = menuItem;
            Parent = null;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            _children = new();
        }
        public HierarchicalMenuItemViewModel(MenuItem menuItem, HierarchicalMenuItemViewModel parent)
        {
            _menuItem = menuItem;
            Parent = parent;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            Parent?.Children.Add(this);

            _children = new();

            CreateExecuteClickCommand();
        }

        public HierarchicalMenuItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
            RaisePropertyChanged(nameof(IsLeaf));
        }

        public HierarchicalMenuItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, HierarchicalMenuItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new((from child in _menuItem.SubMenus
                             select new HierarchicalMenuItemViewModel(eventAggregator, child, this)).ToList<HierarchicalMenuItemViewModel>());

            CreateExecuteClickCommand();

            RaisePropertyChanged(nameof(IsLeaf));
        }
        #endregion

        #region Properties
        public int Id => _menuItem.Id;
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;
        public int Level => _menuItem.Level;
        public string NavigationNam => _menuItem.NavigationName;
        public bool IsSelectedOnInitialize => _menuItem.IsSelectedOnInitialize;
        //   public HierarchicalMenuItemViewModel Parent => _parent;
        public HierarchicalMenuItemViewModel Parent { get; set; }
        protected ObservableCollection<HierarchicalMenuItemViewModel> _children;
        public ObservableCollection<HierarchicalMenuItemViewModel> Children => _children;
        public bool HasTitle => !string.IsNullOrEmpty(_menuItem.Title);
        public bool IsLeaf => (_children is not null) && (_children.Count <= 0);
        public bool IsTopLevelItem => (Parent is null) && IsLeaf;
        public bool IsTopLevelHeader => (Parent is null) && !IsLeaf;
        public bool IsTopLevel => IsTopLevelItem || IsTopLevelHeader;

        public bool IsSubmenuItem => (Parent is not null) && IsLeaf;
        public bool IsSubmenuHeader => (Parent is not null) && !IsLeaf;
        public bool IsSubmenu => IsSubmenuItem || IsSubmenuHeader;
        public string WorkspaceViewEventName { get; set; }
        public bool IsSeparator => _menuItem.IsSeparator;

        protected bool _isTopLevelSelected = false;
        public bool IsTopLevelSelected
        {
            get => _isTopLevelSelected;
            set => SetProperty(ref _isTopLevelSelected, value);
        }

        protected bool _denyPublishWhenIsSelected = false;
        public bool DenyPublishWhenIsSelected
        {
            get => _denyPublishWhenIsSelected;
            set => SetProperty(ref _denyPublishWhenIsSelected, value);
        }

        protected bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (!DenyPublishWhenIsSelected && IsLeaf && _isSelected)
                    {
                        var buildHWorkspaceViewEvent = _eventAggregator.GetEvent(WorkspaceViewEventName) as OnBuildWorkspaceViewEventbase;
                        buildHWorkspaceViewEvent.Publish(new() { CurrentMenuItem = _menuItem });
                    }

                    if (!DenyPublishWhenIsSelected && (IsTopLevelItem || IsSubmenuItem) && _isSelected)
                    {
                        _eventAggregator.GetEvent<OnTopMenuSubSelectedEvent>().Publish(new OnTopMenuSubSelectedEvent { SelectedMenuItem = _menuItem });
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

        #region Click Command
        public ICommand ExecuteClickCommand { get; private set; }

        private void CreateExecuteClickCommand()
        {
            ExecuteClickCommand = new DelegateCommand(() =>
            {
                if (DenyPublishWhenIsSelected)
                {
                    DenyPublishWhenIsSelected = false;

                    var buildHWorkspaceViewEvent = _eventAggregator.GetEvent(WorkspaceViewEventName) as OnBuildWorkspaceViewEventbase;
                    buildHWorkspaceViewEvent.Publish(new() { CurrentMenuItem = _menuItem });
                }
                else
                {
                    IsSelected = true;
                }
            },
            () =>
            {
                var canExecute = true;
                return canExecute;
            });
        }
        #endregion
    }
}