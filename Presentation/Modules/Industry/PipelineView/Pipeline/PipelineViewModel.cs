using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

namespace Aksl.Modules.Pipeline.ViewModels
{
    public class PipelineViewModel : BindableBase
    {
        #region Members
        #endregion

        #region Constructors
        public PipelineViewModel()
        {
           // RaisePropertyChanged(nameof(Direction));
        }
        #endregion

        #region Properties
        private Brush _liquidColor= Brushes.Green;
        public Brush LiquidColor
        {
            get => _liquidColor;
            set => SetProperty<Brush>(ref _liquidColor, value);
        }

        private int _capRadius = 10;
        public int CapRadius
        {
            get => _capRadius;
            set => SetProperty<int>(ref _capRadius, value);
        }

        public List<WaterDirection> WaterDirectionList
        {
            get => Enum.GetValues(typeof(WaterDirection)).Cast<WaterDirection>().ToList();
        }

        private WaterDirection _selectedWaterDirection = WaterDirection.WestToEast;
        public WaterDirection SelectedWaterDirection
        {
            get => _selectedWaterDirection;
            set => SetProperty<WaterDirection>(ref _selectedWaterDirection, value);
        }

        //private WaterDirection _waterDirection = WaterDirection.Pause;
        //public WaterDirection Direction
        //{
        //    get => _waterDirection;
        //    set => SetProperty<WaterDirection>(ref _waterDirection, value);
        //}
        #endregion

        #region Pause Method
        //public void PauseFlow(object sender, RoutedEventArgs e)
        //{
        //    Direction= WaterDirection.Pause;
        //}
        #endregion
    }
}
