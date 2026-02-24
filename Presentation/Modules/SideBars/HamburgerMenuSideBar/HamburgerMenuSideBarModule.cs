using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.HamburgerMenuSideBar.Views;
using Aksl.Modules.HamburgerMenuSideBar.ViewModels;

namespace Aksl.Modules.HamburgerMenuSideBar
{
    public class HamburgerMenuSideBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarModule(IUnityContainer container)
        {
            this._container = container; 
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HamburgerMenuSideBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenuSideBarHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenuSideBarHubViewModel>());
        }
        #endregion
    }
}
