using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Toolkit.Services;
using Aksl.Toolkit.UI;

using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

using Aksl.Modules.Account.Views;

namespace Aksl.Modules.Account.ViewModels
{
    public class LoginViewModel : BindableBase, IDataErrorInfo, INavigationAware
    {
        #region Members
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService;
        private readonly Dictionary<string, string> _errors;
        private object _activeView = default;
        #endregion

        #region Constructors
        public LoginViewModel()
        {
            _regionManager = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IRegionManager>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            _errors = new();

            CreateLoginCommand();
            CreateCloseCommand();

            RegisterPropertyChanged();
        }
        #endregion

        #region Properties
        public string Title { get; private set; } = "Sign In";

        private string _userName;
        [Required(ErrorMessage = "用户名不能为空")]
        [RegularExpression("^[a-zA-Z]{1}([a-zA-Z0-9]){3,15}$", ErrorMessage = "用户名必须是4到16个字母或者\n\r数字,且以字母开头.")]
        public string UserName
        {
            get => _userName;
            set => SetProperty<string>(ref _userName, value);
        }

        private string _password;
        [Required(ErrorMessage = "密码不能为空")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[$@$!%#?&])[a-zA-Z\d$@$!%#?&]{8,}$", ErrorMessage = "密码至少8个字符,必须包含一个字母,\n\r一个数字,一个特殊字符.")]
        public string Password
        {
            get => _password;
            set => SetProperty<string>(ref _password, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
            //{
            //    if (SetProperty<bool>(ref _isLoading, value))
            //    {
            //        (LoginCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            //    }
            //}
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool _isSuccessful = false;
        public bool IsSuccessful
        {
            get => _isSuccessful;
            set => SetProperty(ref _isSuccessful, value);
        }

        public bool HasErrors => _errors.Count > 0;
        #endregion

        #region RegisterPropertyChanged Method
        private void RegisterPropertyChanged()
        {
            this.PropertyChanged += (sender, e) =>
            {
                if (sender is LoginViewModel lvm)
                {
                    if (e.PropertyName == nameof(IsLoading) || e.PropertyName == nameof(UserName) || e.PropertyName == nameof(Password))
                    {
                        (LoginCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    }
                }
            };
        }
        #endregion

        #region Login Command
        public ICommand LoginCommand { get; private set; }

        private void CreateLoginCommand()
        {
            LoginCommand = new DelegateCommand(async () =>
            {
                await ExecuteLoginCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteLoginCommand();
                return canExecute;
            });
        }

        private async Task ExecuteLoginCommandAsync()
        {
            IsLoading = true;

            try
            {
                StatusMessage = "Logining.......";

                RemoveLoginView();

                IsSuccessful = true;

                _eventAggregator.GetEvent<OnSignInedEvent>().Publish(new OnSignInedEvent { UserName = this.UserName, IsSuccessful = true });

                //_eventAggregator.GetEvent<OnTopMenuSelectedEvent>().Publish(new OnTopMenuSelectedEvent
                //{
                //    SelectedMenuItem = new Infrastructure.MenuItem
                //    {
                //        Title = "主页",
                //        Level = 2,
                //        IsHome = true,
                //        IsSelectedOnInitialize = true,
                //        ItemType = MenuItemType.Item,
                //        ModuleName = "HomeModule",
                //        ViewName = "Aksl.Modules.Home.Views.HomeView,Aksl.Modules.Home"
                //    }
                //});

                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Login In Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteLoginCommand()
        {
            return !IsLoading && !HasErrors;
        }
        #endregion

        #region Close Command
        public async void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                VisualTreeFinder visualTreeFinder = new();
                var shellWindow = visualTreeFinder.FindVisualParent<Window>(button);
                var parents = visualTreeFinder.FindVisualParents<FrameworkElement>(button);
                var loginView = parents.FirstOrDefault(e => e is UserControl) as LoginView;
                var childs = visualTreeFinder.FindVisualChilds<FrameworkElement>(loginView);

                IsLoading = true;

                try
                {
                    StatusMessage = "Closing.......";

                    RemoveLoginView();

                    _eventAggregator.GetEvent<OnSignInedEvent>().Publish(new OnSignInedEvent { UserName = "", IsSuccessful = false });

                    await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Close Failure:");
                }

                IsLoading = false;
            }
        }

        public ICommand CloseCommand { get; private set; }

        private void CreateCloseCommand()
        {
            CloseCommand = new DelegateCommand(async () =>
            {
                await ExecuteCloseCommandAsync();
            },
            () =>
            {
                return true;
            });
        }

        private async Task ExecuteCloseCommandAsync()
        {
            IsLoading = true;

            try
            {
                IsSuccessful = false;

                StatusMessage = "Logining.......";

                RemoveLoginView();

                _eventAggregator.GetEvent<OnSignInedEvent>().Publish(new OnSignInedEvent { UserName = "", IsSuccessful = false });

                // await Task.Delay(TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"Please userName and  password", "Login Failure:");
            }

            IsLoading = false;
        }
        #endregion

        #region Loaded Method
        public void RemoveLoginView()
        {
            var contentRegion = _regionManager.Regions[RegionNames.ShellContentRegion];

            Type loginViewType = typeof(LoginView);
            var loginViewName = typeof(LoginView).Name;

            if (contentRegion is not null)
            {
                if (!string.IsNullOrEmpty(loginViewName))
                {
                    var logintView = contentRegion.Views.FirstOrDefault(v => v.GetType() == loginViewType);
                    if (logintView is null)
                    {
                        logintView = contentRegion.GetView(loginViewType.FullName);
                    }

                    if (logintView is not null)
                    {
                        contentRegion.Remove(logintView);
                    }
                }

                if (_activeView is not null)
                {
                    contentRegion.Activate(_activeView);
                }
            }
        }
        #endregion

        #region IDataErrorInfo Interface
        public string this[string columnName]
        {
            get
            {
                ValidationContext validationContext = new(this, null, null);
                validationContext.MemberName = columnName;
                List<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = new();
                var isValidate = Validator.TryValidateProperty(this.GetType().GetProperty(columnName).GetValue(this, null), validationContext, validationResults);
                if (!isValidate && validationResults.Any())
                {
                    if (!_errors.ContainsKey(columnName))
                    {
                        _errors.Add(columnName, "");
                        // return _errors[columnName];
                    }

                    return string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage).ToArray());
                }
                else
                {
                    _errors.Remove(columnName);
                }

                //RaisePropertyChanged(nameof(HasErrors));

                return null;
            }
            set
            {
                _errors[columnName] = value;
            }
        }

        public string Error => null;
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _activeView = navigationContext.Parameters["ActiveView"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            RemoveLoginView();

            _eventAggregator.GetEvent<OnSignInedEvent>().Publish(new OnSignInedEvent { UserName = this.UserName, IsSuccessful = this.IsSuccessful });

            //_eventAggregator.GetEvent<OnTopMenuSelectedEvent>().Publish(new OnTopMenuSelectedEvent
            //{
            //    SelectedMenuItem = new Infrastructure.MenuItem
            //    {
            //        Title = "主页",
            //        Level = 2,
            //        IsHome = true,
            //        IsSelectedOnInitialize = true,
            //        ItemType = MenuItemType.Item,
            //        ModuleName = "HomeModule",
            //        ViewName = "Aksl.Modules.Home.Views.HomeView,Aksl.Modules.Home"
            //    }
            //});
        }
        #endregion
    }
}
