using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aksl.Toolkit.Converters
{
    public sealed class WindowStateMaximizedToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is WindowState && (WindowState)value == WindowState.Maximized) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public sealed class WindowStateMaximizedToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is WindowState && (WindowState)value == WindowState.Maximized) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
