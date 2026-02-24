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

namespace Aksl.Modules.CoolingTower.ViewModels
{
    public class CoolingTowerViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService; 
        #endregion

        #region Constructors
        public CoolingTowerViewModel()
        {
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
        }
        #endregion

        #region Properties
        public IEnumerable<TowerStatus> TowerStatusList
        {
            get => Enum.GetValues(typeof(TowerStatus)).Cast<TowerStatus>().ToList();
        }

        private TowerStatus _selectedTowerStatus = TowerStatus.Normal;
        public TowerStatus SelectedTowerStatus
        {
            get => _selectedTowerStatus;
            set => SetProperty<TowerStatus>(ref _selectedTowerStatus, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        #endregion
    }
}
