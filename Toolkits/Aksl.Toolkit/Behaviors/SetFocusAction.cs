using System.Windows;
using System.Windows.Input;

//JulMar.Windows.Actions
namespace Aksl.Xaml.Behaviors
{
    public class SetFocusAction : Microsoft.Xaml.Behaviors.TriggerAction<FrameworkElement>
    {
        #region Dependency TargetProperty
        /// <summary>
        /// Dependency Property backing the Target property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
                        DependencyProperty.Register(nameof(Target), typeof(FrameworkElement), typeof(SetFocusAction), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// This property allows you to set the focus target independent of where this
        /// action is applied - so you can apply the trigger/action to the Window and then
        /// push focus to a child element as an example.
        /// </summary>
        public FrameworkElement Target
        {
            get => (FrameworkElement)base.GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }
        #endregion

        #region Dependency TargetProperty
        protected override void Invoke(object parameter)
        {
            var element = Target ?? AssociatedObject;

            if (element is not null)
            {
                // If unable to directly set focus, then attempt to set *logical* focus
                // to our element so that when/if focus returns to this focus scope we will have focus.
                if (!element.Focus())
                {
                    var fs = FocusManager.GetFocusScope(element);
                    FocusManager.SetFocusedElement(fs, element);
                }
            }
        }
        #endregion
    }
}
