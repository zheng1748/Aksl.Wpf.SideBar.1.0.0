using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

using Aksl.Windows.Extensions;

namespace Aksl.Modules.Shell.Views
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanMaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanRestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));

            if (Icon == null)
            {
                SetDefaultWindowIcon();
            }

            var windowChrome = new WindowChrome
            {
                CaptionHeight=48, 
                CornerRadius =new CornerRadius(2),
                GlassFrameThickness = new Thickness(-1),
                ResizeBorderThickness = new Thickness(2),
                UseAeroCaptionButtons = false,
                NonClientFrameEdges= NonClientFrameEdges.None
            };
            WindowChrome.SetWindowChrome(this, windowChrome);
        }

        #region Set WindowIcon Method
        public System.Drawing.Icon? WindowIcon
        {
            get
            {
                WindowInteropHelper interopHelper = new WindowInteropHelper(this);
                var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                //System.Windows.Forms.Application.ExecutablePath
                return System.Drawing.Icon.ExtractAssociatedIcon(exePath!);
            }
        }

        public void SetDefaultWindowIcon()
        {
            Icon = WindowIcon?.ToBitmap().ToImageSource();
            //System.Drawing.Icon icon = new System.Drawing.Icon("ApplicationIcon.ico");
            //InteropMethods.SendMessage(new WindowInteropHelper(this).Handle, 0x80/*WM_SETICON*/, 1 /*ICON_LARGE*/, icon.Handle);
        }

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
        private void CanMaximizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip) && WindowState != WindowState.Maximized;
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

        private void CanRestoreWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip) && WindowState == WindowState.Maximized;
        }

        //private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        //}

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
