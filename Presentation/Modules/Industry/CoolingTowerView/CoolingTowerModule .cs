using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.CoolingTower.ViewModels;
using Aksl.Modules.CoolingTower.Views;

namespace Aksl.Modules.CoolingTower
{
    public class CoolingTowerModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public CoolingTowerModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<CoolingTowerHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(CoolingTowerView).ToString(),
                                               () => this._container.Resolve<CoolingTowerViewModel>());

            ViewModelLocationProvider.Register(typeof(CoolingTowerHubView).ToString(),
                                              () => this._container.Resolve<CoolingTowerHubViewModel>());
        }
        #endregion
    }
}
