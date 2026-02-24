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
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;

using Aksl.Toolkit.Services;

namespace Aksl.Modules.LiveCharts.Axes.ViewModels
{
    public class DateTimeScaledViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IDialogViewService _dialogViewService;
        private AxisPosition _selectedPosition = AxisPosition.End;
        private int _selectedColor = 0;
        private readonly LvcColor[] _colors = ColorPalletes.FluentDesign;
        #endregion

        #region Constructors
        public DateTimeScaledViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
        }
        #endregion

        #region Properties
        public ISeries[] Series { get; set; } =
        [
            new ColumnSeries<DateTimePoint>
            {
                Values =
                [
                    new() { DateTime = new(2021, 1, 1), Value = 3 },
                    new() { DateTime = new(2021, 1, 2), Value = 6 },
                    new() { DateTime = new(2021, 1, 3), Value = 5 },
                    new() { DateTime = new(2021, 1, 4), Value = 3 },
                    new() { DateTime = new(2021, 1, 5), Value = 5 },
                    new() { DateTime = new(2021, 1, 6), Value = 8 },
                    new() { DateTime = new(2021, 1, 7), Value = 6 }
                ]
            }
        ];

        public ICartesianAxis[] XAxes { get; set; } =
        [
            new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MM-dd"))
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
