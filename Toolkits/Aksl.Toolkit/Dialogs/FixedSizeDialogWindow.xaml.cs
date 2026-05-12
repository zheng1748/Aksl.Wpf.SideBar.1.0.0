using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;

using Prism.Services.Dialogs;

namespace Aksl.Dialogs.Views
{
    public partial class FixedSizeDialogWindow : Window, IDialogWindow
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWindow"/> class.
        /// </summary>
        public FixedSizeDialogWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));

            var windowChrome = new WindowChrome
            {
                CaptionHeight = 48,
                CornerRadius = new CornerRadius(2),
                GlassFrameThickness = new Thickness(-1),
                ResizeBorderThickness = new Thickness(2),
                UseAeroCaptionButtons = false,
                NonClientFrameEdges = NonClientFrameEdges.None
            };
            WindowChrome.SetWindowChrome(this, windowChrome);
        }
        #endregion

        #region IDialogWindow
        public new object Content
        {
            get => ContentPair.Content;
            set => ContentPair.Content = value;
        }

        /// <summary>
        /// The <see cref="IDialogResult"/> of the dialog.
        /// </summary>
        public IDialogResult Result { get; set; }
        #endregion

        #region Window Command Methods
        public void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        public void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        public void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        public void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        public void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element is null)
            {
                return;
            }

            var point = WindowState == WindowState.Maximized ? new System.Windows.Point(0, element.ActualHeight) :
                                                               new System.Windows.Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }
        #endregion
    }
}
