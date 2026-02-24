using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.Views
{
    public partial class MenuItemHeader : UserControl
    {
        public MenuItemHeader()
        {
            InitializeComponent();
        }

        #region Properties
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty
             = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MenuItemHeader), new PropertyMetadata(defaultValue: null, propertyChangedCallback: null));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuItemHeader menuItemHeader)
            {
                if (e.NewValue is string newValue && e.OldValue is string oldValue)
                {
                    if (newValue != oldValue)
                    {
                        var headerTextBlock = (TextBlock)menuItemHeader.FindName("headerTextBlock");
                        if (headerTextBlock is not null)
                        {
                            headerTextBlock.Text = newValue;
                        }
                    }
                }
            }

            //if (e.NewValue is string value)
            //{
            //    if (d is FrameworkElement element)
            //    {
            //        var headerTextBlock = (TextBlock)element.FindName("headerTextBlock");
            //        if (headerTextBlock is not null)
            //        {
            //            headerTextBlock.Text = value;
            //        }
            //    }
            //};
        }
        #endregion
    }
}
