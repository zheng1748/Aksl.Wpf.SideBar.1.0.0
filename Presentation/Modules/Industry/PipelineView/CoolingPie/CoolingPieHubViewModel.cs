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

namespace Aksl.Modules.Pipeline.ViewModels
{
    public class CoolingPieHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IDialogViewService _dialogViewService;
        #endregion

        #region Constructors
        public CoolingPieHubViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            CoolingPieViewModel = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<CoolingPieViewModel>();

            CreateTurnCommand();
            CreatePauseCommand();

            RegisterPropertyChanged();

            // RaisePropertyChanged(nameof(CoolingPieViewModel.Status));
        }
        #endregion

        #region Properties
        public CoolingPieViewModel CoolingPieViewModel { get; private set; }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty<bool>(ref _isLoading, value))
                {
                    (TurnCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    (PauseCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool Playing { get; set; } = false;
        #endregion

        #region RegisterPropertyChanged Method
        private void RegisterPropertyChanged()
        {
            CoolingPieViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is CoolingPieViewModel clvm)
                {
                    if (e.PropertyName == nameof(CoolingPieViewModel.SelectedFanStatus))
                    {
                        (TurnCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        (PauseCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    }
                }
            };
        }
        #endregion

        #region Turn Command
        public ICommand TurnCommand { get; private set; }

        private void CreateTurnCommand()
        {
            TurnCommand = new DelegateCommand(async () =>
            {
                await ExecuteTurnCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteTurnCommand();
                return canExecute;
            });
        }

        private async Task ExecuteTurnCommandAsync()
        {
            IsLoading = true;

            try
            {
               // StatusMessage = "Turning.......";

                CoolingPieViewModel.SelectedFanStatus = FanStatus.Turn;

                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Turn Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteTurnCommand()
        {
            return !IsLoading && !(CoolingPieViewModel.SelectedFanStatus == FanStatus.Turn);
        }
        #endregion

        #region Pause Command
        public ICommand PauseCommand { get; private set; }

        private void CreatePauseCommand()
        {
            PauseCommand = new DelegateCommand(async () =>
            {
                await ExecutePauseCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecutePauseCommand();
                return canExecute;
            });
        }

        private async Task ExecutePauseCommandAsync()
        {
            IsLoading = true;

            try
            {
               // StatusMessage = "Pauseing.......";

                CoolingPieViewModel.SelectedFanStatus = FanStatus.Pause;

                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Pause Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecutePauseCommand()
        {
            return !IsLoading && !(CoolingPieViewModel.SelectedFanStatus == FanStatus.Pause);
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
