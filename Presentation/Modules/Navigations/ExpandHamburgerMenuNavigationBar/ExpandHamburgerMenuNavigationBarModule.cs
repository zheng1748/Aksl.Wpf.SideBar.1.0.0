using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.ExpandHamburgerMenuNavigationBar.Views;
using Aksl.Modules.ExpandHamburgerMenuNavigationBar.ViewModels;

namespace Aksl.Modules.ExpandHamburgerMenuNavigationBar
{
    public class ExpandHamburgerMenuNavigationBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public ExpandHamburgerMenuNavigationBarModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ExpandHamburgerMenuNavigationBarHubView>();

            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ExpandHamburgerMenuNavigationBarHubView).ToString(),
                                               () => this._container.Resolve<ExpandHamburgerMenuNavigationBarHubViewModel>());

        }
        #endregion
    }
}
