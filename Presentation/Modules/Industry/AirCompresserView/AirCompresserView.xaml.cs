using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aksl.Modules.AirCompresser.Views
{
    public partial class AirCompresserView : UserControl
    {
        public AirCompresserView()
        {
            InitializeComponent();
        }

        #region CompressStatus Property

        public static readonly DependencyProperty CompressStatusProperty =
                                                  DependencyProperty.Register("CompressStatus", typeof(CompressStatus), typeof(AirCompresserView), new PropertyMetadata(defaultValue: CompressStatus.Normal, propertyChangedCallback: OnCompressStatusChanged));
        public CompressStatus CompressStatus
        {
            get { return (CompressStatus)GetValue(CompressStatusProperty); }
            set { SetValue(CompressStatusProperty, value); }
        }

        private static void OnCompressStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AirCompresserView airCompresserView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                }

                if (airCompresserView.DataContext is ViewModels.AirCompresserViewModel airCompresserViewModel)
                {

                }
            }
        }
        #endregion
    }
}
