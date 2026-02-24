using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.Thermometer.ViewModels;
using Aksl.Modules.Thermometer.Views;

namespace Aksl.Modules.Thermometer
{
    public class ThermometerModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public ThermometerModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ThermometerHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ThermometerView).ToString(),
                                               () => this._container.Resolve<ThermometerViewModel>());

            ViewModelLocationProvider.Register(typeof(ThermometerHubView).ToString(),
                                               () => this._container.Resolve<ThermometerHubViewModel>());
        }
        #endregion
    }
}
