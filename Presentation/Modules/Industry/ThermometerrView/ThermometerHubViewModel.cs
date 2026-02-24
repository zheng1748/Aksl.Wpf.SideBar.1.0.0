using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Infrastructure;
using Aksl.Toolkit.Services;
using Aksl.Infrastructure.Events;

namespace Aksl.Modules.Thermometer.ViewModels
{
    public class ThermometerHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService; 
        #endregion

        #region Constructors
        public ThermometerHubViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            ThermometerViewModel = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<ThermometerViewModel>();

            CreateTemperatureCommand();

            RegisterPropertyChanged();

            //RaisePropertyChanged(nameof(ThermometerViewModel.TemperatureValue));
        }
        #endregion

        #region Properties
        public ThermometerViewModel ThermometerViewModel { get; private set; }

        private bool _isLoading=false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty<bool>(ref _isLoading, value))
                {
                    (TemperatureCommand as DelegateCommand<object>)?.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region RegisterPropertyChanged Method
        private void RegisterPropertyChanged()
        {
            ThermometerViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is ThermometerViewModel tvm)
                {
                    if (e.PropertyName == nameof(ThermometerViewModel.TemperatureValue))
                    {
                        (TemperatureCommand as DelegateCommand<object>)?.RaiseCanExecuteChanged();
                    }
                }
            };
        }
        #endregion

        #region Temperature Command
        public ICommand TemperatureCommand { get; private set; }

        private void CreateTemperatureCommand()
        {
            TemperatureCommand = new DelegateCommand<object>(async (o) =>
            {
                await ExecuteTemperatureCommandAsync(o);
            },
            (o) =>
            {
                var canExecute = CanExecuteTemperatureCommand(o);
                return canExecute;
            });
        }

        private async Task ExecuteTemperatureCommandAsync(object tempValue)
        {
            IsLoading = true;

            try
            {
                if ((short)tempValue is short tv)
                {
                    ThermometerViewModel.TemperatureValue = tv;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Temperature Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteTemperatureCommand(object tempValue)
        {
            return !IsLoading && !((tempValue is not null) && (ThermometerViewModel.TemperatureValue == (short)tempValue));
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
