using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

using Aksl.Toolkit.Services;

namespace Aksl.Modules.LiveCharts.Bars.ViewModels
{
    public class AutoUpdateViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IDialogViewService _dialogViewService;
        private readonly Random _random = new();
        #endregion

        #region Constructors
        public AutoUpdateViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            ObservablePoints = 
            [
                new() { X = 0, Y = 2 },
                new() { X = 1, Y = 5 },
                new() { X = 2, Y = 4 }
            ];

            Series = 
            [
                new ColumnSeries<ObservablePoint>(ObservablePoints)
            ];
        }
        #endregion

        #region Properties
        public ObservableCollection<ISeries> Series { get; set; }
        public ObservableCollection<ObservablePoint> ObservablePoints { get; set; }
        #endregion

        #region Methods
        public void AddItemClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AddItem();
        }

        private void AddItem()
        {
            // Because the Series property is an ObservableCollection, // mark
            // the chart will listen for changes and update // mark
            // in this case a new series is added to the chart // mark

            var newColumnSeries = new ColumnSeries<int>(FetchVales());
            Series.Add(newColumnSeries);
        }

        public void RemoveItemClick()
        {
            if (Series.Count == 1) return;

            // This will also remove the series from the UI. // mark
            Series.RemoveAt(Series.Count - 1);
        }

        public void ReplaceItemClick()
        {
            var randomIndex = _random.Next(0, ObservablePoints.Count - 1);

            // The chart will update the point at the specified index // mark
            ObservablePoints[randomIndex] = new(ObservablePoints[randomIndex].X, _random.Next(1, 10));
        }

        public void AddSeriesClick()
        {
            var newPoint = new ObservablePoint
            {
                X = ObservablePoints.Count,
                Y = _random.Next(0, 10)
            };

            // The new point will be drawn at the end of the chart // mark
            ObservablePoints.Add(newPoint);
        }

        public void RemoveSeriesClick()
        {
            if (ObservablePoints.Count < 2) return;

            // Because the ObservablePoints property is an ObservableCollection, // mark
            // the chart will listen for changes and update // mark
            // in this case a point is removed from the chart // mark

            ObservablePoints.RemoveAt(0);
        }

        private bool? _isStreaming = false;
        public async void ConstantChangesClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _isStreaming = _isStreaming is null ? true : !_isStreaming;

            while (_isStreaming.Value)
            {
                RemoveItemClick();
                AddItem();
                await Task.Delay(1000);
            }
        }

        private int[] FetchVales()
        {
            return
            [
                _random.Next(0, 10),
                _random.Next(0, 10),
                _random.Next(0, 10)
            ];
        }
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
