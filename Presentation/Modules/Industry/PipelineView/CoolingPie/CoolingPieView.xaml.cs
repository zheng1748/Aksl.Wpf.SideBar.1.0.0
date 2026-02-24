using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aksl.Modules.Pipeline.Views
{
    public partial class CoolingPieView : UserControl
    {
        public CoolingPieView()
        {
            InitializeComponent();

            PathGeometry pathGeometry = new PathGeometry();
            PathFigureCollection figures = PathFigureCollection.Parse("M 10,100 C 35,0 135,0 160,100 180,190 285,200 310,100");
            pathGeometry.Figures = figures;

            PointAnimationUsingPath pointAnimation = new PointAnimationUsingPath();
            pointAnimation.Duration = TimeSpan.FromSeconds(5);
            pointAnimation.RepeatBehavior = RepeatBehavior.Forever;
            pointAnimation.AutoReverse = true;
            pointAnimation.PathGeometry = pathGeometry;
        }

        #region FanStatus Property

        public static readonly DependencyProperty FanStatusProperty =
                                                  DependencyProperty.Register("FanStatus", typeof(FanStatus), typeof(CoolingPieView), new PropertyMetadata(defaultValue: FanStatus.Pause, propertyChangedCallback: OnFanStatusChanged));
        public FanStatus FanStatus
        {
            get { return (FanStatus)GetValue(FanStatusProperty); }
            set { SetValue(FanStatusProperty, value); }
        }
        #endregion

        #region TurnDirection Property

        public static readonly DependencyProperty TurnDirectionProperty =
                                     DependencyProperty.Register("TurnDirection", typeof(TurnDirection), typeof(CoolingPieView), new PropertyMetadata(defaultValue: TurnDirection.Clockwise, propertyChangedCallback: OnFanStatusChanged));
        public TurnDirection TurnDirection
        {
            get { return (TurnDirection)GetValue(TurnDirectionProperty); }
            set { SetValue(TurnDirectionProperty, value); }
        }

        private static void OnFanStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CoolingPieView coolingPieView)
            {
                string state;

                if (coolingPieView.FanStatus == FanStatus.Turn)
                {
                    state = "Turn";
                    state += coolingPieView.TurnDirection == TurnDirection.Clockwise ? "Clockwise" : "Counterclockwise";
                }
                else
                {
                    state = "Pause";
                }

                coolingPieView.VisualState = state;
            }
        }
        #endregion

        #region CenterX Property

        public static readonly DependencyProperty VisualStateProperty =
                                                  DependencyProperty.Register("VisualState", typeof(string), typeof(CoolingPieView), new PropertyMetadata(defaultValue: "TurnClockwise", propertyChangedCallback: null));
        public string VisualState
        {
            get { return (string)GetValue(VisualStateProperty); }
            set { SetValue(VisualStateProperty, value); }
        }
        #endregion

        #region CenterX Property

        public static readonly DependencyProperty CenterXProperty =
                                                  DependencyProperty.Register("CenterX", typeof(int), typeof(CoolingPieView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnCenterChanged));
        public int CenterX
        {
            get { return (int)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CoolingPieView coolingPieView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                }

                if (coolingPieView.DataContext is ViewModels.CoolingPieViewModel coolingPieViewModel)
                {

                }
            }
        }
        #endregion

        #region CenterY Property

        public static readonly DependencyProperty CenterYProperty =
                                                  DependencyProperty.Register("CenterY", typeof(int), typeof(CoolingPieView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnCenterChanged));
        public int CenterY
        {
            get { return (int)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }
        #endregion

    }
}
