using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.HamburgerMenuPopupSideBar.Views;
using Aksl.Modules.HamburgerMenuPopupSideBar.ViewModels;

namespace Aksl.Modules.HamburgerMenuPopupSideBar
{
    public class HamburgerMenuPopupSideBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuPopupSideBarModule(IUnityContainer container)
        {
            this._container = container; 
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HamburgerMenPopupSideBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenPopupSideBarHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenPopupSideBarHubViewModel>());
        }
        #endregion
    }
}
