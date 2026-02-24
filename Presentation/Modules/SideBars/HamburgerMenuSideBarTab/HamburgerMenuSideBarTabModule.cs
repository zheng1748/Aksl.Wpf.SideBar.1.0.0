using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.HamburgerMenuSideBarTab.Views;
using Aksl.Modules.HamburgerMenuSideBarTab.ViewModels;

namespace Aksl.Modules.HamburgerMenuSideBarTab
{
    public class HamburgerMenuSideBarTabModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuSideBarTabModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HamburgerMenuSideBarTabHubView>();
         //  containerRegistry.RegisterForNavigation<IndustryHamburgerMenuTabHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenuSideBarTabHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenuSideBarTabHubViewModel>()); ;

            //ViewModelLocationProvider.Register(typeof(IndustryHamburgerMenuTabHubView).ToString(),
            //                               () => this._container.Resolve<IndustryHamburgerMenuTabHubViewModel>());

        }
        #endregion
    }
}
