using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.ExpandHamburgerMenuSub.Views;
using Aksl.Modules.ExpandHamburgerMenuSub.ViewModels;

namespace Aksl.Modules.ExpandHamburgerMenuSub
{
    public class ExpandHamburgerMenuSubModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public ExpandHamburgerMenuSubModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ExpandHamburgerMenuSubHubView>();

        
            containerRegistry.RegisterForNavigation<DropsExpandHamburgerMenuSubHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ExpandHamburgerMenuSubHubView).ToString(),
                                               () => this._container.Resolve<ExpandHamburgerMenuSubHubViewModel>());

            ViewModelLocationProvider.Register(typeof(DropsExpandHamburgerMenuSubHubView).ToString(),
                                           () => this._container.Resolve<DropsExpandHamburgerMenuSubHubViewModel>());
        }
        #endregion
    }
}
