using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class PipelineView : UserControl
    {
        public PipelineView()
        {
            InitializeComponent();
        }

        #region WaterDirection Property

        public static readonly DependencyProperty WaterDirectionProperty =
                                              DependencyProperty.Register("WaterDirection", typeof(WaterDirection), typeof(Aksl.Modules.Pipeline.Views.PipelineView), new PropertyMetadata(defaultValue: WaterDirection.Pause, propertyChangedCallback: null));

        public WaterDirection WaterDirection
        {
            get => (WaterDirection)GetValue(WaterDirectionProperty);
            set => SetValue(WaterDirectionProperty, value);
        }

        private static void OnWaterDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PipelineView pipelineView)
            {
                if ((WaterDirection)e.NewValue != (WaterDirection)e.OldValue)
                {
                    //pipelineView.WaterDirection = (WaterDirection)e.NewValue;

                    if (pipelineView.DataContext is ViewModels.PipelineViewModel pipelineViewModel)
                    {
                       // pipelineViewModel.Direction = (WaterDirection)e.NewValue;
                    }
                }
            }
        }
        #endregion

        #region LiquidColor Property

        public static readonly DependencyProperty LiquidColorProperty =
                                                  DependencyProperty.Register("LiquidColor", typeof(Brush), typeof(PipelineView), new PropertyMetadata(defaultValue: Brushes.Orange, propertyChangedCallback: null));

        public Brush LiquidColor
        {
            get => (Brush)GetValue(LiquidColorProperty);
            set => SetValue(LiquidColorProperty, value);
        }

        private static void OnLiquidColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PipelineView pipelineView)
            {
                if ((Brush)e.NewValue != (Brush)e.OldValue)
                {
                    if (pipelineView.DataContext is ViewModels.PipelineViewModel pipelineViewModel)
                    {
                        
                    }
                }
            }
        }
        #endregion

        #region CapRadius Property

        public static readonly DependencyProperty CapRadiusProperty =
                                                  DependencyProperty.Register("CapRadius", typeof(int), typeof(PipelineView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnCapRadiusChanged));
        public int CapRadius
        {
            get => (int)GetValue(CapRadiusProperty);
            set => SetValue(CapRadiusProperty, value);
        }
        private static void OnCapRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PipelineView pipelineView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                    if (pipelineView.DataContext is ViewModels.PipelineViewModel pipelineViewModel)
                    {

                    }
                }
            }
        }
        #endregion
    }
}
