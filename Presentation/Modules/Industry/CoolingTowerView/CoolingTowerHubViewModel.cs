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

using Aksl.Toolkit.Services;

namespace Aksl.Modules.CoolingTower.ViewModels
{
    public class CoolingTowerHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IDialogViewService _dialogViewService;
        #endregion

        #region Constructors
        public CoolingTowerHubViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            CoolingTowerViewModel = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<CoolingTowerViewModel>();

            CreateNormalCommand();
            CreateErrorCommand();

            RegisterPropertyChanged();
        }
        #endregion

        #region Properties
        public CoolingTowerViewModel CoolingTowerViewModel { get; private set; }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty<bool>(ref _isLoading, value))
                {
                    (NormalCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    (ErrorCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region RegisterPropertyChanged Method
        private void RegisterPropertyChanged()
        {
            CoolingTowerViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is CoolingTowerViewModel cltvm)
                {
                    if (e.PropertyName == nameof(CoolingTowerViewModel.SelectedTowerStatus))
                    {
                        (NormalCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                        (ErrorCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                    }
                }
            };
        }
        #endregion

        #region Normal Command
        public ICommand NormalCommand { get; private set; }

        private void CreateNormalCommand()
        {
            NormalCommand = new DelegateCommand(async () =>
            {
                await ExecuteNormalCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteNormalCommand();
                return canExecute;
            });
        }

        private async Task ExecuteNormalCommandAsync()
        {
            IsLoading = true;

            try
            {
                // StatusMessage = "Turning.......";

                CoolingTowerViewModel.SelectedTowerStatus= TowerStatus.Normal;

                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Normal Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteNormalCommand()
        {
            return !IsLoading && !(CoolingTowerViewModel.SelectedTowerStatus == TowerStatus.Normal);
        }
        #endregion

        #region Error Command
        public ICommand ErrorCommand { get; private set; }

        private void CreateErrorCommand()
        {
            ErrorCommand = new DelegateCommand(async () =>
            {
                await ExecuteErrorCommandAsync();
            },
            () =>
            {
                var canExecute = CanExecuteErrorCommand();
                return canExecute;
            });
        }

        private async Task ExecuteErrorCommandAsync()
        {
            IsLoading = true;

            try
            {
                // StatusMessage = "Pauseing.......";

                CoolingTowerViewModel.SelectedTowerStatus = TowerStatus.Error;

                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _dialogViewService.AlertWhenAsync($"{ex.Message}", "Error Failure:");
            }

            IsLoading = false;
        }

        private bool CanExecuteErrorCommand()
        {
            return !IsLoading && !(CoolingTowerViewModel.SelectedTowerStatus == TowerStatus.Error);
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
