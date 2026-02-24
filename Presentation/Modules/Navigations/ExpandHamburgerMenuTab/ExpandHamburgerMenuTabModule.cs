using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Unity;

using Aksl.Modules.ExpandHamburgerMenuTab.Views;
using Aksl.Modules.ExpandHamburgerMenuTab.ViewModels;

namespace Aksl.Modules.ExpandHamburgerMenuTab
{
    public class ExpandHamburgerMenuTabModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public ExpandHamburgerMenuTabModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ExpandHamburgerMenuTabHubView>();

           containerRegistry.RegisterForNavigation<AxesExpandHamburgerMenuTabHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ExpandHamburgerMenuTabHubView).ToString(),
                                               () => this._container.Resolve<ExpandHamburgerMenuTabHubViewModel>()); ;

            ViewModelLocationProvider.Register(typeof(AxesExpandHamburgerMenuTabHubView).ToString(),
                                            () => this._container.Resolve<AxesExpandHamburgerMenuTabHubViewModel>());
        }
        #endregion
    }
}
