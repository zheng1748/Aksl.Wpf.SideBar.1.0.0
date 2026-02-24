using System;
using System.Windows;
using System.Windows.Input;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

using Aksl.Toolkit.Controls;

namespace Aksl.Modules.HamburgerMenuSideBarTab.ViewModels
{
    public class TabItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        private readonly TabInformation _tabInformation;
        #endregion

        #region Constructors
        public TabItemViewModel(TabInformation tabInformation)
        {
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            _tabInformation = tabInformation;

            CloseTabButtonVisibility = _tabInformation.CloseTabButtonVisibility;

            CreateExecuteCloseTabCommand();
        }
        #endregion

        #region Properties
        public TabInformation TabInformation => _tabInformation;
        public string Name => _tabInformation.Name;
        public string Title => _tabInformation.Title;
        public string ViewName => _tabInformation.ViewName;

        private Type _viewElementType = default;
        public Type ViewElementType
        {
            get
            {
                if (_viewElementType is null)
                {
                    string viewTypeAssemblyQualifiedName = _tabInformation.ViewName;
                    _viewElementType = Type.GetType(viewTypeAssemblyQualifiedName);
                }

                return _viewElementType;
            }
        }

        private DependencyObject _viewElement = default;
        public DependencyObject ViewElement
        {
            get
            {
                if (_viewElement is null)
                {
                    if (ViewElementType is not null)
                    {
                        // viewElemen = Activator.CreateInstance(viewType) as DependencyObject;
                        _viewElement = (PrismApplication.Current as PrismApplicationBase).Container.Resolve(ViewElementType) as DependencyObject;
                    }
                }

                return _viewElement;
            }
            set
            {
                SetProperty<DependencyObject>(ref _viewElement, value);
            }
        }

        public PackIconKind IconKind
        {
            get
            {
                PackIconKind kind = PackIconKind.None;

                _ = Enum.TryParse(_tabInformation.IconKind, out kind);

                return kind;
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (_isSelected)
                    {
                        _eventAggregator.GetEvent<OnActiveTabItemEvent>().Publish(new() { SelectedTabItem = _tabInformation });
                    }
                }
            }
        }

        private Visibility _closeTabButtonVisibility= Visibility.Visible;
        public Visibility CloseTabButtonVisibility
        {
            get => _closeTabButtonVisibility;
            set => SetProperty<Visibility>(ref _closeTabButtonVisibility, value);
        }
        #endregion

        #region CloseTab Command
        public event EventHandler RequestClose;
        public ICommand ExecuteCloseTabCommand { get; private set; }

        private void CreateExecuteCloseTabCommand()
        {
            ExecuteCloseTabCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(this, EventArgs.Empty);
            },
            () =>
            {
                return true;
            });
        }
        #endregion
    }
}
