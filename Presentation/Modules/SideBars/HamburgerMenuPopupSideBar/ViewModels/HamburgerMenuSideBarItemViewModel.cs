using System;
using System.Collections.ObjectModel;
using System.Linq;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;
using Aksl.Toolkit.Controls;

namespace Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels
{
    public partial class HamburgerMenuSideBarItemViewModel : BindableBase
    {
        #region Members 
        private readonly IUnityContainer _container;
        protected readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        protected readonly HamburgerMenuSideBarItemViewModel _parent;
        protected ObservableCollection<HamburgerMenuSideBarItemViewModel> _children;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarItemViewModel()
        {
            _menuItem = null;
            Parent = null;

            _children = new();
        }

        public HamburgerMenuSideBarItemViewModel(MenuItem menuItem, HamburgerMenuSideBarItemViewModel parent)
        {
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _menuService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IMenuService>();

            _menuItem = menuItem;
            Parent = parent;

            Parent?.Children.Add(this);

            _children = new();

            ThePopupViewModel = new();

            CreatePopupSideBarItemModelsAsync().GetAwaiter().GetResult();
        }

        public HamburgerMenuSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
            RaisePropertyChanged(nameof(IsLeaf));
        }

        public HamburgerMenuSideBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, HamburgerMenuSideBarItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new((from child in _menuItem.SubMenus
                             select new HamburgerMenuSideBarItemViewModel(eventAggregator, child, this)).ToList<HamburgerMenuSideBarItemViewModel>());

            RaisePropertyChanged(nameof(IsLeaf));
        }
        #endregion

        #region Properties
        public MenuItem MenuItem => _menuItem;
        //public string IconPath => _menuItem.IconPath;
        public string Name => _menuItem.Name;
        public string Title => _menuItem.Title;

        private string _workspaceViewEventName = default;
        public string WorkspaceViewEventName 
        {
            get => _workspaceViewEventName;
            set
            {
                if (SetProperty<string>(ref _workspaceViewEventName, value))
                {
                    foreach (var hsmi in ThePopupViewModel.AllLeafPopupSideBarItems)
                    {
                        hsmi.WorkspaceViewEventName = _workspaceViewEventName;
                    }
                }
            }
        }
        public int Level => _menuItem.Level;
        public string NavigationNam => _menuItem.NavigationName;
        public bool IsSelectedOnInitialize => _menuItem.IsSelectedOnInitialize;
        public HamburgerMenuSideBarItemViewModel Parent { get; set; }
        public ObservableCollection<HamburgerMenuSideBarItemViewModel> Children => _children;
        public bool HasChildren => (_children is not null) && _children.Any();
        public bool HasTitle => !string.IsNullOrEmpty(_menuItem.Title);
        public bool IsLeaf => (_children is not null) && _children.Count <= 0;
        public bool HasSubMenu => ThePopupViewModel.AllLeafPopupSideBarItems.Any();

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (IsLeaf && _isSelected && !HasSubMenu)
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
