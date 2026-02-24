using System;
using System.Linq;
using System.Threading.Tasks;
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
using Aksl.Infrastructure;
using Aksl.Infrastructure.Events;

using Aksl.Modules.Account.Views;

namespace Aksl.Modules.Account.ViewModels
{
    public class LoginStatusViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService;
        private object _activeView = default;
        #endregion

        #region Constructors
        public LoginStatusViewModel()
        {
            _regionManager = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IRegionManager>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            CreateSignInCommand();
            CreateSignOutCommand();

            RegisterSignInedEvent();

            Title = "Please Login";
        }
        #endregion

        #region Properties
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty<string>(ref _title, value);
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty<string>(ref _userName, value);
        }

        private bool _isSignIning = false;
        public bool IsSignIning
        {
            get => _isSignIning;
            set
            {
                if (SetProperty<bool>(ref _isSignIning, value))
                {
                    (SignInCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isAuthenticated = false;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty<bool>(ref _isAuthenticated, value);
        }
        #endregion

        #region Register SignIned Event
        private void RegisterSignInedEvent()
        {
            _eventAggregator.GetEvent<OnSignInedEvent>().Subscribe((siet) =>
            {
                IsSignIning = false;

                UserName = siet.UserName;
                IsAuthenticated = siet.IsSuccessful;

                //RaisePropertyChanged(nameof(Title));
                Title = null;
                Title = IsAuthenticated ? $"{UserName} Login" : "Please Login";

            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Get Active View Method
        private void GetActiveView()
        {
            var regionManager = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IRegionManager>();
            var region = regionManager.Regions[RegionNames.ShellContentRegion];

            if (region is not null)
            {
                var activeViews = region.ActiveViews;
                if (activeViews is not null && activeViews.Any())
                {
                    _activeView = activeViews.FirstOrDefault();
                }
            }
        }
        #endregion

        #region SignIn Command
        public ICommand SignInCommand { get; private set; }

        private void CreateSignInCommand()
        {
            SignInCommand = new DelegateCommand(async () =>
            {
                await ExecutSignInCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteSignInCommand();
                return canExecute;
            });
        }

        private async Task ExecutSignInCommandAsync()
        {
            IsSignIning = true;

            try
            {
                var contentRegion = _regionManager.Regions[RegionNames.ShellContentRegion];

                Type loginViewType = typeof(LoginView);
                var loginViewName = nameof(LoginView);

                if (contentRegion is not null && !string.IsNullOrEmpty(loginViewName))
                {
                    var currentView = contentRegion.Views.FirstOrDefault(v => v.GetType() == loginViewType);

                    var activeViews = contentRegion.ActiveViews;
                    if (activeViews is not null && activeViews.Any())
                    {
                        _activeView = activeViews.FirstOrDefault();
                    }

                    NavigationParameters navigationParameters = new()
                    {
                       { "ActiveView",  _activeView }
                    };

                    _regionManager.RequestNavigate(RegionNames.ShellContentRegion, loginViewName, navigationParameters);

                    // currentView = region.Views.FirstOrDefault(v => v.GetType() == loginViewType);
                    //if (currentView is null)
                    //{
                    //    currentView = region.GetView(loginViewType.FullName);
                    //}

                    //if (currentView is not null)
                    //{
                    //    region.Add(currentView);
                    //}

                    //IsAuthenticated = true;
                }
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Sign In Failure:");
            }

            //  IsSignIning = false;
        }

        private bool CanExecuteSignInCommand()
        {
            return !IsSignIning;
        }
        #endregion

        #region SignOut Command
        public ICommand SignOutCommand { get; private set; }

        private void CreateSignOutCommand()
        {
            SignOutCommand = new DelegateCommand(async () =>
            {
                await ExecutSignOutCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteSignOutCommand();
                return canExecute;
            });
        }

        private async Task ExecutSignOutCommandAsync()
        {
            IsSignIning = true;

            try
            {
                IsAuthenticated = false;

                Title = null;
                Title = "Please Login";

                RaisePropertyChanged(nameof(UserName));
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Sign In Failure:");
            }

            IsSignIning = false;
        }

        private bool CanExecuteSignOutCommand()
        {
            return !IsSignIning;
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
