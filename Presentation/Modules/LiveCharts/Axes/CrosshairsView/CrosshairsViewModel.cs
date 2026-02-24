using System;
using System.Collections.ObjectModel;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;

using Aksl.Toolkit.Services;

namespace Aksl.Modules.LiveCharts.Axes.ViewModels
{
    public class CrosshairsViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IDialogViewService _dialogViewService;
        private AxisPosition _selectedPosition = AxisPosition.End;
        private int _selectedColor = 0;
        private readonly LvcColor[] _colors = ColorPalletes.FluentDesign;
        #endregion

        #region Constructors
        public CrosshairsViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
        }
        #endregion

        #region Properties
        public ISeries[] Series { get; set; } =
        [
           new LineSeries<double> { Values = [200, 558, 458, 249, 457, 339, 587] },
           new LineSeries<double> { Values = [210, 400, 300, 350, 219, 323, 618] },
        ];

        public Axis[] XAxes { get; set; } =
        [
           new Axis
           {
               CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
               CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
               CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
               Labeler = value => value.ToString("N2")
           }
         ];

        public Axis[] YAxes { get; set; } =
        [
            new Axis
            {
                CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
                CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
                CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
                CrosshairSnapEnabled = true // snapping is also supported
            }
        ];
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
