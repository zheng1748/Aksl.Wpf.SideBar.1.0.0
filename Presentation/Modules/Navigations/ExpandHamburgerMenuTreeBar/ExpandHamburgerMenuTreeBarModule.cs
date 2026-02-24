using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.ExpandHamburgerMenuTreeBar.Views;
using Aksl.Modules.ExpandHamburgerMenuTreeBar.ViewModels;

namespace Aksl.Modules.ExpandHamburgerMenuTreeBar
{
    public class ExpandHamburgerMenuTreeBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public ExpandHamburgerMenuTreeBarModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        { 
            containerRegistry.RegisterForNavigation<ExpandHamburgerMenuTreeBarHubView>();

            containerRegistry.RegisterForNavigation<AnimationsExpandHamburgerMenuTreeBarHubView>();
            containerRegistry.RegisterForNavigation<GraphicsExpandHamburgerMenuTreeBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ExpandHamburgerMenuTreeBarHubView).ToString(),
                                               () => this._container.Resolve<ExpandHamburgerMenuTreeBarHubViewModel>());

            ViewModelLocationProvider.Register(typeof(AnimationsExpandHamburgerMenuTreeBarHubView).ToString(),
                                              () => this._container.Resolve<AnimationsExpandHamburgerMenuTreeBarHubViewModel>());
            ViewModelLocationProvider.Register(typeof(GraphicsExpandHamburgerMenuTreeBarHubView).ToString(),
                                              () => this._container.Resolve<GraphicsExpandHamburgerMenuTreeBarHubViewModel>());
        }
        #endregion
    }
}
