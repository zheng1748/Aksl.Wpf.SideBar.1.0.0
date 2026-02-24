using Unity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

using Aksl.Infrastructure;
using Aksl.Modules.Account.Views;
using Aksl.Modules.HamburgerMenuSideBar.Views;
using Aksl.Modules.HamburgerMenuNavigationSideBar.Views;
using Aksl.Modules.HamburgerMenuPopupSideBar;
using Aksl.Modules.HamburgerMenuPopupSideBar.Views;

namespace Aksl.Modules.Shell
{
    public class ShellModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        #endregion

        #region Constructors
        public ShellModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
          
           //_regionManager.RequestNavigate(RegionNames.ShellContentRegion, nameof(Aksl.Modules.HamburgerMenuSideBar.Views.HamburgerMenuSideBarHubView));
       // _regionManager.RequestNavigate(RegionNames.ShellContentRegion, nameof(HamburgerMenuNavigationSideBarHubView));
        //   _regionManager.RequestNavigate(RegionNames.ShellContentRegion, nameof(Aksl.Modules.HamburgerMenuTreeSideBar.Views.HamburgerMenuTreeSideBarHubView));

           _regionManager.RequestNavigate(RegionNames.ShellContentRegion, nameof(Aksl.Modules.HamburgerMenuPopupSideBar.Views.HamburgerMenPopupSideBarHubView));

            _regionManager.RequestNavigate(RegionNames.ShellLoginRegion, nameof(LoginStatusView));
        }
        #endregion
    }
}
