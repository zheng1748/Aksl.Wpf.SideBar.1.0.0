using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Unity;

using Aksl.Modules.HamburgerMenuNavigationSideBar.Views;
using Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar
{
    public class HamburgerMenuNavigationSideBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuNavigationSideBarModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HamburgerMenuNavigationSideBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenuNavigationSideBarHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenuNavigationSideBarHubViewModel>());
        }
        #endregion
    }
}
