using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aksl.Toolkit.Converters
{
    public sealed class BooleanNegationToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is bool && !((bool)value)) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is Visibility && (Visibility)value == Visibility.Collapsed;
    }
}
