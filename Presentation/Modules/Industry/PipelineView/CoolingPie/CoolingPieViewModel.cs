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
using Aksl.Toolkit;

namespace Aksl.Modules.Pipeline.ViewModels
{
    public class CoolingPieViewModel : BindableBase
    {
        #region Members
        #endregion

        #region Constructors
        public CoolingPieViewModel()
        {
            //  RaisePropertyChanged(nameof(Status));
            GetVisualState();
        }
        #endregion

        #region Properties
        private int _centerX = 20;
        public int CenterX
        {
            get => _centerX;
            set => SetProperty<int>(ref _centerX, value);
        }

        private int _centerY = 20;
        public int CenterY
        {
            get => _centerY;
            set => SetProperty<int>(ref _centerY, value);
        }

        public List<FanStatus> FanStatusList
        {
            get => Enum.GetValues(typeof(FanStatus)).Cast<FanStatus>().ToList();
        }

        private FanStatus _selectedFanStatus = FanStatus.Turn;
        public FanStatus SelectedFanStatus
        {
            get => _selectedFanStatus;
            set
            {
                if (SetProperty<FanStatus>(ref _selectedFanStatus, value))
                {
                    VisualState = GetVisualState();
                }
            }
        }

        public List<TurnDirection> TurnDirectionList
        {
            get => Enum.GetValues(typeof(TurnDirection)).Cast<TurnDirection>().ToList();
        }

        private TurnDirection _selectedTurnDirection =  TurnDirection.Clockwise;
        public TurnDirection SelectedTurnDirection
        {
            get => _selectedTurnDirection;
            set
            {
                if (SetProperty<TurnDirection>(ref _selectedTurnDirection, value))
                {
                    VisualState = GetVisualState();
                }
            }
        }

        private string _visualState;
        public string VisualState
        {
            get => _visualState;
            set => SetProperty<string>(ref _visualState, value);
        }
        #endregion

        #region Get State Method

        private bool IsClockwise
        {
            get
            {
                return SelectedTurnDirection switch
                {
                    TurnDirection.Clockwise => true,
                    _ => false
                };
            }
        }

        protected  string GetVisualState()
        {
            string state;

            if (SelectedFanStatus== FanStatus.Turn)
            {
                state = "Turn";
                state += IsClockwise ? "Clockwise" : "Counterclockwise";
            }
            else
            {
                state = "Pause";
            }

            return state;
        }
        #endregion
    }
}
