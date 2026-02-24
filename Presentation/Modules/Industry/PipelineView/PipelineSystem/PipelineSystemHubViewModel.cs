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
    public class PipelineSystemHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService; 
        #endregion

        #region Constructors
        public PipelineSystemHubViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            TopPipeline = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<PipelineViewModel>();
            RightPipeline = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<PipelineViewModel>();
            BottomPipeline = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<PipelineViewModel>();
            LeftPipeline = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<PipelineViewModel>();

            TopLeftCoolingPie = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<CoolingPieViewModel>();
            TopRightCoolingPie = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<CoolingPieViewModel>();
            BottomLeftCoolingPie = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<CoolingPieViewModel>();
            BottomRightCoolingPie = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<CoolingPieViewModel>();

            CreateWestToEastFlowCommand();
            CreateEastToWestFlowCommand();
            CreatePauseFlowCommand();

            //RegisterPropertyChanged(TopPipeline);
            //RegisterPropertyChanged(RightPipeline);
            //RegisterPropertyChanged(BottomPipeline);
            //RegisterPropertyChanged(LeftPipeline);

            TopPipeline.SelectedWaterDirection = WaterDirection.WestToEast;
            RightPipeline.SelectedWaterDirection = WaterDirection.WestToEast;

            BottomPipeline.SelectedWaterDirection = WaterDirection.EastToWest;
            LeftPipeline.SelectedWaterDirection = WaterDirection.EastToWest;
        }
        #endregion

        #region Properties
        public PipelineViewModel TopPipeline { get; private set; }
        public PipelineViewModel RightPipeline { get; private set; }
        public PipelineViewModel BottomPipeline { get; private set; }
        public PipelineViewModel LeftPipeline { get; private set; }

        public CoolingPieViewModel TopLeftCoolingPie { get; private set; }
        public CoolingPieViewModel TopRightCoolingPie { get; private set; }
        public CoolingPieViewModel BottomLeftCoolingPie { get; private set; }
        public CoolingPieViewModel BottomRightCoolingPie { get; private set; }

        private bool _isLoading=false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty<bool>(ref _isLoading, value))
                {
                    (WestToEastFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    (EastToWestFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    (PauseFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region RegisterPropertyChanged Method
        private void RegisterPropertyChanged(PipelineViewModel pipelineViewModel)
        {
            pipelineViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is PipelineViewModel plvm)
                {
                    if (e.PropertyName == nameof(PipelineViewModel.SelectedWaterDirection))
                    {
                        (WestToEastFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        (EastToWestFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        (PauseFlowCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    }
                }
            };
        }
        #endregion

        #region WestToEast Flow Command
        public ICommand WestToEastFlowCommand { get; private set; }

        private void CreateWestToEastFlowCommand()
        {
            WestToEastFlowCommand = new DelegateCommand(async () =>
            {
                await ExecuteWestToEastFlowCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteWestToEastFlowCommand();
                return canExecute;
            });
        }

        private async Task ExecuteWestToEastFlowCommandAsync()
        {
            IsLoading = true;

            try
            {
                TopPipeline.SelectedWaterDirection= WaterDirection.WestToEast;
                RightPipeline.SelectedWaterDirection = WaterDirection.WestToEast;

                BottomPipeline.SelectedWaterDirection = WaterDirection.EastToWest;
                LeftPipeline.SelectedWaterDirection = WaterDirection.EastToWest;

                TopLeftCoolingPie.SelectedFanStatus = FanStatus.Turn;
                TopRightCoolingPie.SelectedFanStatus = FanStatus.Turn;
                BottomLeftCoolingPie.SelectedFanStatus = FanStatus.Turn;
                BottomRightCoolingPie.SelectedFanStatus = FanStatus.Turn;

                TopLeftCoolingPie.SelectedTurnDirection = TurnDirection.Clockwise;
                TopRightCoolingPie.SelectedTurnDirection = TurnDirection.Clockwise;
                BottomLeftCoolingPie.SelectedTurnDirection = TurnDirection.Clockwise;
                BottomRightCoolingPie.SelectedTurnDirection = TurnDirection.Clockwise;

                await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "West To East Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteWestToEastFlowCommand()
        {
            return !IsLoading  && !(TopPipeline.SelectedWaterDirection == WaterDirection.WestToEast);
        }
        #endregion

        #region EastToWest Flow Command
        public ICommand EastToWestFlowCommand { get; private set; }

        private void CreateEastToWestFlowCommand()
        {
            EastToWestFlowCommand = new DelegateCommand(async () =>
            {
                await ExecuteEastToWestFlowCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteEastToWestFlowCommand();
                return canExecute;
            });
        }

        private async Task ExecuteEastToWestFlowCommandAsync()
        {
            IsLoading = true;

            try
            {
                TopPipeline.SelectedWaterDirection = WaterDirection.EastToWest;
                RightPipeline.SelectedWaterDirection = WaterDirection.EastToWest;

                BottomPipeline.SelectedWaterDirection = WaterDirection.WestToEast;
                LeftPipeline.SelectedWaterDirection = WaterDirection.WestToEast;

                TopLeftCoolingPie.SelectedFanStatus = FanStatus.Turn;
                TopRightCoolingPie.SelectedFanStatus = FanStatus.Turn;
                BottomLeftCoolingPie.SelectedFanStatus = FanStatus.Turn;
                BottomRightCoolingPie.SelectedFanStatus = FanStatus.Turn;

                TopLeftCoolingPie.SelectedTurnDirection = TurnDirection.Counterclockwise;
                TopRightCoolingPie.SelectedTurnDirection = TurnDirection.Counterclockwise;
                BottomLeftCoolingPie.SelectedTurnDirection = TurnDirection.Counterclockwise;
                BottomRightCoolingPie.SelectedTurnDirection = TurnDirection.Counterclockwise;

                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "East To West Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteEastToWestFlowCommand()
        {
            return !IsLoading && !(TopPipeline.SelectedWaterDirection == WaterDirection.EastToWest);
        }
        #endregion

        #region Pause Flow Command
        public ICommand PauseFlowCommand { get; private set; }

        private void CreatePauseFlowCommand()
        {
            PauseFlowCommand = new DelegateCommand(async () =>
            {
                await ExecutePauseFlowCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecutePauseFlowCommand();
                return canExecute;
            });
        }

        private async Task ExecutePauseFlowCommandAsync()
        {
            IsLoading = true;

            try
            {

                TopPipeline.SelectedWaterDirection =  WaterDirection.Pause;
                RightPipeline.SelectedWaterDirection = WaterDirection.Pause;

                BottomPipeline.SelectedWaterDirection = WaterDirection.Pause;
                LeftPipeline.SelectedWaterDirection = WaterDirection.Pause;

                TopLeftCoolingPie.SelectedFanStatus = FanStatus.Pause;
                TopRightCoolingPie.SelectedFanStatus = FanStatus.Pause;
                BottomLeftCoolingPie.SelectedFanStatus = FanStatus.Pause;
                BottomRightCoolingPie.SelectedFanStatus = FanStatus.Pause;

                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Pause Flow Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecutePauseFlowCommand()
        {
            return !IsLoading && !(TopPipeline.SelectedWaterDirection is WaterDirection.Pause);
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
