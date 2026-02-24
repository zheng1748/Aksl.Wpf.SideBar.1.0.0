using System.Windows;
using System.Windows.Controls;

namespace Aksl.Xaml.Behaviors
{
    public static class PasswordManager
    {
        #region Attached Password Property

        public static DependencyProperty PasswordProperty = 
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordManager), new PropertyMetadata(defaultValue: null, propertyChangedCallback: null));

        public static string GetPassword(DependencyObject obj) => (string)obj.GetValue(PasswordProperty);

        public static void SetPassword(DependencyObject obj, string value) => obj.SetValue(PasswordProperty, value);
        #endregion

        #region Attached AutoPassword Property

        public static DependencyProperty AutoPasswordProperty =
             DependencyProperty.RegisterAttached("AutoPassword", typeof(bool), typeof(PasswordManager), new PropertyMetadata(defaultValue: false, propertyChangedCallback: OnAutoPasswordChanged));
        public static bool GetAutoPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoPasswordProperty);
        }

        public static void SetAutoPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoPasswordProperty, value);
        }

        private static void OnAutoPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue && d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= OnPasswordChanged;

                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += OnPasswordChanged;
                }
            }
        }

        private static void OnPasswordChanged(object d, RoutedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if (!string.Equals(GetPassword(passwordBox), passwordBox.Password))
                {
                    SetPassword(passwordBox, passwordBox.Password);
                }
            }
        }
        #endregion
    }
}
