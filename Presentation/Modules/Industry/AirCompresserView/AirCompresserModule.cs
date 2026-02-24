using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.AirCompresser.ViewModels;
using Aksl.Modules.AirCompresser.Views;

namespace Aksl.Modules.AirCompresser
{
    public class AirCompresserModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public AirCompresserModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AirCompresserHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(AirCompresserView).ToString(),
                                               () => this._container.Resolve<AirCompresserViewModel>());

            ViewModelLocationProvider.Register(typeof(AirCompresserHubView).ToString(),
                                              () => this._container.Resolve<AirCompresserHubViewModel>());
        }
        #endregion
    }
}
