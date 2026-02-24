using System.Windows;
using System.Windows.Controls;

namespace Aksl.Xaml.Behaviors
{
    public class PasswordBoxBehavior : Microsoft.Xaml.Behaviors.Behavior<PasswordBox>
    {
        #region OnAttached/OnDetaching Method
        protected override void OnAttached()
        {
            base.OnAttached();

            var passwordBox = AssociatedObject;

            if (passwordBox is not null) 
            {
                passwordBox.PasswordChanged += OnPasswordChanged;
            }
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
               var password= PasswordManager.GetPassword(passwordBox);

                if (!string.Equals(password, passwordBox.Password))
                {
                    PasswordManager.SetPassword(passwordBox, passwordBox.Password);
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            var passwordBox = AssociatedObject;

            if (passwordBox is not null)
            {
                passwordBox.PasswordChanged -= OnPasswordChanged;
            }
        }
        #endregion
    }
}
