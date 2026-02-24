using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.HamburgerMenuTreeSideBar.Views;
using Aksl.Modules.HamburgerMenuTreeSideBar.ViewModels;

namespace Aksl.Modules.HamburgerMenuTreeSideBar 
{
    public class HamburgerMenuTreeSideBarViewModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public HamburgerMenuTreeSideBarViewModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        { 
            containerRegistry.RegisterForNavigation<HamburgerMenuTreeSideBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HamburgerMenuTreeSideBarHubView).ToString(),
                                               () => this._container.Resolve<HamburgerMenuTreeSideBarHubViewModel>());
        }
        #endregion
    }
}
