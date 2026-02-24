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

namespace Aksl.Modules.CoolingTower.Views
{
    public partial class CoolingTowerView : UserControl
    {
        public CoolingTowerView()
        {
            InitializeComponent();
        }

        #region TowerStatus Property

        public static readonly DependencyProperty TowerStatusProperty =
                                      DependencyProperty.Register("TowerStatus", typeof(TowerStatus), typeof(CoolingTowerView), new PropertyMetadata(defaultValue: TowerStatus.Normal, propertyChangedCallback: null));
        public TowerStatus TowerStatus
        {
            get { return (TowerStatus)GetValue(TowerStatusProperty); }
            set { SetValue(TowerStatusProperty, value); }
        }

        private static void OnTowerStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CoolingTowerView coolingTowerView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                }

                if (coolingTowerView.DataContext is ViewModels.CoolingTowerViewModel coolingTowerViewModel)
                {

                }
            }
        }
        #endregion
    }
}
