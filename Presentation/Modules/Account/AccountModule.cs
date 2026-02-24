using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Unity;

using Aksl.Modules.Account.ViewModels;
using Aksl.Modules.Account.Views;

namespace Aksl.Modules.Account
{
    public class AccountModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public AccountModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoginStatusView>();
            containerRegistry.RegisterForNavigation<LoginView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(LoginStatusView).ToString(),
                                              () => this._container.Resolve<LoginStatusViewModel>());
            ViewModelLocationProvider.Register(typeof(LoginView).ToString(),
                                             () => this._container.Resolve<LoginViewModel>());
        }
        #endregion
    }
}
