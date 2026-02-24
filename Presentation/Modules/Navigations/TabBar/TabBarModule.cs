using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Modules.TabBar.Views;
using Aksl.Modules.TabBar.ViewModels;

namespace Aksl.Modules.TabBar 
{
    public class TabBarModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public TabBarModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TabBarHubView>();

            containerRegistry.RegisterForNavigation<PipelinesTabBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(TabBarHubView).ToString(),
                                               () => this._container.Resolve<TabBarHubViewModel>()); ;

            ViewModelLocationProvider.Register(typeof(PipelinesTabBarHubView).ToString(),
                                               () => this._container.Resolve<PipelinesTabBarHubViewModel>());
        }
        #endregion
    }
}
