using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.HamburgerMenuTab.Views;
using Aksl.Modules.HamburgerMenuTab.ViewModels;

namespace Aksl.Modules.HamburgerMenuTab
{
    public class HamburgerMenuTabModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuTabModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HamburgerMenuTabHubView>();

            containerRegistry.RegisterForNavigation<BlursHamburgerMenuTabHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenuTabHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenuTabHubViewModel>()); ;

            ViewModelLocationProvider.Register(typeof(BlursHamburgerMenuTabHubView).ToString(),
                                           () => this._container.Resolve<BlursHamburgerMenuTabHubViewModel>());

        }
        #endregion
    }
}
