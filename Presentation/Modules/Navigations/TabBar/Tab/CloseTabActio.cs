using Microsoft.Xaml.Behaviors;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

using Aksl.Toolkit.UI;

using Aksl.Modules.TabBar.ViewModels;

//https://wiki.bananeatomic.fr/index.php?title=Prism_navigation_with_tabcontrol&action=edit

namespace Aksl.Modules.TabBar
{
    public sealed class CloseTabAction : TriggerAction<Button>
    {
        protected override void Invoke(object parameter)
        {
            VisualTreeFinder visualTreeFinder = new();

            var args = parameter as RoutedEventArgs;
            if (args is null)
            {
                return;
            }

            var tabItem = visualTreeFinder.FindVisualParent<TabItem>(args.OriginalSource as DependencyObject);
            if (tabItem is null)
            {
                return;
            }

            var tabControl = visualTreeFinder.FindVisualParent<TabControl>(tabItem);
            if (tabControl is null)
            {
                return;
            }

            if (tabItem.DataContext is TabItemViewModel tabItemViewModel)
            {
                if (tabControl.DataContext is TabViewModel tabViewModel)
                {
                    tabViewModel.Remove(tabItemViewModel);
                }
            }
        }
    }
}
