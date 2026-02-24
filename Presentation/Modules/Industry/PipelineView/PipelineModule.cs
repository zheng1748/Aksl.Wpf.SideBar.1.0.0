using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.Pipeline.ViewModels;
using Aksl.Modules.Pipeline.Views;

namespace Aksl.Modules.Pipeline
{
    public class PipelineModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public PipelineModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<PipelineHubView>();
            containerRegistry.RegisterForNavigation<CoolingPieHubView>();
            containerRegistry.RegisterForNavigation<PipelineSystemHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(PipelineHubView).ToString(),
                                               () => this._container.Resolve<PipelineHubViewModel>());
            ViewModelLocationProvider.Register(typeof(CoolingPieHubView).ToString(),
                                              () => this._container.Resolve<CoolingPieHubViewModel>());
            ViewModelLocationProvider.Register(typeof(PipelineSystemHubView).ToString(),
                                              () => this._container.Resolve<PipelineSystemHubViewModel>());
        }
        #endregion
    }
}
