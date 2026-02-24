using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aksl.Modules.Pipeline.Converters
{
    public class IntToCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new CornerRadius(topLeft: (int)value, topRight: (int)value, bottomRight: (int)value, bottomLeft: (int)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
