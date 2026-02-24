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

namespace Aksl.Modules.AirCompresser.ViewModels
{
    public class AirCompresserViewModel : BindableBase
    {
        #region Members
        #endregion

        #region Constructors
        public AirCompresserViewModel()
        {
        }
        #endregion

        #region Properties
        public IEnumerable<CompressStatus> CompressStatusList
        {
            get => Enum.GetValues(typeof(CompressStatus)).Cast<CompressStatus>().ToList();
        }

        private CompressStatus _selectedCompressStatus = CompressStatus.Normal;
        public CompressStatus SelectedCompressStatus
        {
            get => _selectedCompressStatus;
            set => SetProperty<CompressStatus>(ref _selectedCompressStatus, value);
        }
        #endregion
    }
}
