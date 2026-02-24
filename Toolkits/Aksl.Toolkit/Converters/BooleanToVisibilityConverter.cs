using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aksl.Toolkit.Converters
{
    /// <summary>
    /// 将 true 转换为 <see cref="Visibility.Visible"/> ,将 false 转换为
    /// <see cref="Visibility.Collapsed"/> 的值转换器。
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is Visibility && (Visibility)value == Visibility.Visible;
    }
}
