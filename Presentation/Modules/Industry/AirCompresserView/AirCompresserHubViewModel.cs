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

namespace Aksl.Modules.AirCompresser.ViewModels
{
    public class AirCompresserHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IDialogViewService _dialogViewService;
        #endregion

        #region Constructors
        public AirCompresserHubViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            AirCompresserViewModel = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<AirCompresserViewModel>();

            CreateNormalCommand();
            CreateErrorCommand();

            RegisterPropertyChanged();

            Title = "Air Compresser";
        }
        #endregion

        #region Properties
        public string Title { get; private set; }
        public AirCompresserViewModel AirCompresserViewModel { get; private set; }

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
            AirCompresserViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is AirCompresserViewModel cltvm)
                {
                    if (e.PropertyName == nameof(AirCompresserViewModel.SelectedCompressStatus))
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

                AirCompresserViewModel.SelectedCompressStatus=  CompressStatus.Normal;

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
            return !IsLoading && !(AirCompresserViewModel.SelectedCompressStatus == CompressStatus.Normal);
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

                AirCompresserViewModel.SelectedCompressStatus = CompressStatus.Error;

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
            return !IsLoading && !(AirCompresserViewModel.SelectedCompressStatus == CompressStatus.Error);
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
