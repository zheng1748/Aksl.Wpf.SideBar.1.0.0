using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Aksl.Modules.Thermometer.Views
{
    public partial class ThermometerView : UserControl
    {
        public ThermometerView()
        {
            InitializeComponent();
        }

        #region Minmum Property

        public static readonly DependencyProperty MinmumProperty =
                                                  DependencyProperty.Register("Minmum", typeof(int), typeof(ThermometerView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnMinmumsChanged));
        public int Minmum
        {
            get => (int)GetValue(MinmumProperty);
            set => SetValue(MinmumProperty, value);
        }

        private static void OnMinmumsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThermometerView thermometerView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                    thermometerView.RefreshComponet();
                }
            }
        }
        #endregion

        #region Maxmum Property

        public static readonly DependencyProperty MaxmumProperty =
                                                  DependencyProperty.Register("Maxmum", typeof(int), typeof(ThermometerView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnMaxmumChanged));
        public int Maxmum
        {
            get => (int)GetValue(MaxmumProperty);
            set => SetValue(MaxmumProperty, value);
        }

        private static void OnMaxmumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThermometerView thermometerView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                    thermometerView.RefreshComponet();
                }
            }
        }
        #endregion

        #region Value Property

        public static readonly DependencyProperty ValueProperty =
                                                  DependencyProperty.Register("Value", typeof(double), typeof(ThermometerView), new PropertyMetadata(defaultValue: 0d, propertyChangedCallback: OnValueChanged));
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThermometerView thermometerView)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    thermometerView.RefreshComponet();
                }

                if (thermometerView.DataContext is ViewModels.ThermometerViewModel thermometerViewModel)
                {

                }
            }
        }
        #endregion

        #region Refresh Componet Method
        private void RefreshComponet()
        {
            // 两种方式触发：尺寸变化、区间变化
            var h = this.MainCanvas.ActualHeight;//通过这个判断界面元素是否加载
            if (h == 0)
            {
                return;
            }
            double w = 75;
            // 类型
            double stepCount = Maxmum - Minmum;// 在这个区间内多少个间隔
            step = h / (Maxmum - Minmum);// 每个间隔距离

            this.MainCanvas.Children.Clear();

            for (int i = 0; i <= stepCount; i++)
            {
                Line line = new()
                {
                    Y1 = i * step,
                    Y2 = i * step,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                this.MainCanvas.Children.Add(line);

                if (i % 10 == 0)
                {
                    line.X1 = 15;
                    line.X2 = w - 15;

                    // 添加文字
                    TextBlock text = new()
                    {
                        Text = (Maxmum - i).ToString(),
                        Width = 20,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 9,
                        Margin = new Thickness(0, -5, -4, 0)
                    };
                    Canvas.SetLeft(text, w - 15);
                    Canvas.SetTop(text, i * step);
                    this.MainCanvas.Children.Add(text);

                    // 添加文字
                    text = new()
                    {
                        Text = (Maxmum - i).ToString(),
                        Width = 20,
                        TextAlignment = TextAlignment.Center,
                        FontSize = 9,
                        Margin = new(-4, -5, 0, 0)
                    };
                    Canvas.SetLeft(text, 0);
                    Canvas.SetTop(text, i * step);
                    this.MainCanvas.Children.Add(text);
                }
                else if (i % 5 == 0)
                {
                    line.X1 = 20;
                    line.X2 = w - 20;
                }
                else
                {
                    line.X1 = 25;
                    line.X2 = w - 25;
                }
            }

            ValueChanged();
        }
        #endregion

        #region Value Changed Method

        private double step = 10;
        private void ValueChanged()
        {
            // 限定值的变化范围 
            //var value = this.Value;
            //if (this.Value < this.Minmum)
            //    value = this.Minmum;
            //if (this.Value > this.Maxmum)
            //    value = this.Maxmum;
            _ = Math.Max(Value, Minmum);
            double value = Math.Min(Value, Maxmum);
            //
            // 温度值与Border的高度的一个转换
            var newValue = value - Minmum;
            newValue *= step;
            newValue += 20;

            // 动画
            DoubleAnimation doubleAnimation = new(newValue, TimeSpan.FromMilliseconds(500));
            this.BorValue.BeginAnimation(HeightProperty, doubleAnimation);
        }
        #endregion
    }
}
