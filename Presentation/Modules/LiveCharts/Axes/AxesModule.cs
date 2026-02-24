using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

using Aksl.Modules.LiveCharts.Axes.ViewModels;
using Aksl.Modules.LiveCharts.Axes.Views;

//install-package LiveChartsCore.SkiaSharpView.WPF -Version 2.0.0-beta.90
//install-package LiveCharts.Core -Version 2.0.0-beta.90

namespace Aksl.Modules.LiveCharts.Axes
{
    public class AxesModule : IModule
    {
        #region Members
        private readonly IUnityContainer _container;
        #endregion

        #region Constructors
        public AxesModule(IUnityContainer container)
        {
            this._container = container;
        }
        #endregion

        #region IModule
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ColorsAndPositionView>();
            containerRegistry.RegisterForNavigation<CrosshairsView>();
            containerRegistry.RegisterForNavigation<CustomSeparatorsIntervalView>();
            containerRegistry.RegisterForNavigation<DateTimeScaledView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(ColorsAndPositionView).ToString(),
                                               () => this._container.Resolve<ColorsAndPositionViewModel>());
            ViewModelLocationProvider.Register(typeof(CrosshairsView).ToString(),
                                               () => this._container.Resolve<CrosshairsViewModel>()); 
            ViewModelLocationProvider.Register(typeof(CustomSeparatorsIntervalView).ToString(),
                                               () => this._container.Resolve<CustomSeparatorsIntervalViewModel>());
            ViewModelLocationProvider.Register(typeof(DateTimeScaledView).ToString(),
                                               () => this._container.Resolve<DateTimeScaledViewModel>());
        }
        #endregion
    }
}
