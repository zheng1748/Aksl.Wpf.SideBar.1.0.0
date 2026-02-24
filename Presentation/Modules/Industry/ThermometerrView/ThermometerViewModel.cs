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

namespace Aksl.Modules.Thermometer.ViewModels
{
    public class ThermometerViewModel : BindableBase
    {
        #region Members

        #endregion

        #region Constructors
        public ThermometerViewModel()
        {
        }
        #endregion

        #region Properties
        private int _minmum=-10;
        public int Minmum
        {
            get => _minmum;
            set => SetProperty<int>(ref _minmum, value);
        }

        private int _maxmum=60;
        public int Maxmum
        {
            get => _maxmum;
            set => SetProperty<int>(ref _maxmum, value);
        }

        private double _temperatureValue=10;
        public double TemperatureValue
        {
            get => _temperatureValue;
            set => SetProperty<double>(ref _temperatureValue, value);
        }
        #endregion
    }
}
