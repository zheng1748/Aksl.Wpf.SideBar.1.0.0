
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Unity;

using Aksl.Modules.LiveCharts.Bars.ViewModels;
using Aksl.Modules.LiveCharts.Bars.Views;

//install-package LiveChartsCore.SkiaSharpView.WPF -Version 2.0.0-beta.90

namespace Aksl.Modules.LiveCharts.Bars
{
    public class BarsModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public BarsModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AutoUpdateView>();
            containerRegistry.RegisterForNavigation<BarsBasicView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(AutoUpdateView).ToString(),
                                               () => this._container.Resolve<AutoUpdateViewModel>());
            ViewModelLocationProvider.Register(typeof(BarsBasicView).ToString(),
                                              () => this._container.Resolve<BarsBasicViewModel>());
        }
        #endregion
    }
}
