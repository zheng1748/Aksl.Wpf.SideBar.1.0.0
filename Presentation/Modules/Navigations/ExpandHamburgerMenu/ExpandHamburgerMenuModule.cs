using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.ExpandHamburgerMenu.Views;
using Aksl.Modules.ExpandHamburgerMenu.ViewModels;

namespace Aksl.Modules.ExpandHamburgerMenu 
{
    public class ExpandHamburgerMenuModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public ExpandHamburgerMenuModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ExpandHamburgerMenuHubView>();

            containerRegistry.RegisterForNavigation<AxesExpandHamburgerMenuHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ExpandHamburgerMenuHubView).ToString(),
                                               () => this._container.Resolve<ExpandHamburgerMenuHubViewModel>());

            ViewModelLocationProvider.Register(typeof(AxesExpandHamburgerMenuHubView).ToString(),
                                             () => this._container.Resolve<AxesExpandHamburgerMenuHubViewModel>());
        }
        #endregion
    }
}
