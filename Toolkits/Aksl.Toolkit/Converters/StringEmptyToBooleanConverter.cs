using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aksl.Toolkit.Converters
{
    public sealed class StringEmptyToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrEmpty((string)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
