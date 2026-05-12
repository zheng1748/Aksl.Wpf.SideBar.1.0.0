using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;

using Prism;
using Prism.Common;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Prism.Unity;
using Unity;

using Aksl.Dialogs.Views;

namespace Aksl.Dialogs.Services
{
    public class DialogService
    {
        private readonly IUnityContainer _container;

        public DialogService()
        {
            _container = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IUnityContainer>();
        }

        public void ShowDialog(FrameworkElement dialogContent, IDialogParameters parameters, string windowName= nameof(FixedSizeDialogWindow), Action<IDialogResult> callback = null)
        {
            ShowDialogInternal(dialogContent, parameters, true, windowName, callback);
        }

        private void ShowDialogInternal(FrameworkElement dialogContent, IDialogParameters parameters, bool isModal, string windowName = null, Action<IDialogResult> callback = null)
        {
            if (parameters == null)
            {
                parameters = new DialogParameters();
            }

            IDialogWindow dialogWindow = CreateDialogWindow(windowName);
            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent(dialogContent, dialogWindow, parameters);

            ShowDialogWindow(dialogWindow, isModal);
        }

        protected virtual void ShowDialogWindow(IDialogWindow dialogWindow, bool isModal)
        {
            if (isModal)
            {
                dialogWindow.ShowDialog();
            }
            else
            {
                dialogWindow.Show();
            }
        }

        protected virtual IDialogWindow CreateDialogWindow(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return _container.Resolve<IDialogWindow>();
            }
            else
            {
                return _container.Resolve<IDialogWindow>(name);
            }
        }

        protected virtual void ConfigureDialogWindowContent(FrameworkElement dialogContent, IDialogWindow window, IDialogParameters parameters)
        {
            //var content = _containerExtension.Resolve<object>(dialogName);
            //if (!(content is FrameworkElement dialogContent))
            //    throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            MvvmHelpers.AutowireViewModel(dialogContent);

            if (dialogContent.DataContext is not IDialogAware viewModel)
            {
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");
            }

            ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
        }

        protected virtual void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, Action<IDialogResult> callback)
        {
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = (o) =>
            {
                dialogWindow.Result = o;
                dialogWindow.Close();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                dialogWindow.Loaded -= loadedHandler;
                dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
            };
            dialogWindow.Loaded += loadedHandler;

            CancelEventHandler closingHandler = null;
            closingHandler = (o, e) =>
            {
                if (!dialogWindow.GetDialogViewModel().CanCloseDialog())
                    e.Cancel = true;
            };
            dialogWindow.Closing += closingHandler;

            EventHandler closedHandler = null;
            closedHandler = (o, e) =>
                {
                    dialogWindow.Closed -= closedHandler;
                    dialogWindow.Closing -= closingHandler;
                    dialogWindow.GetDialogViewModel().RequestClose -= requestCloseHandler;

                    dialogWindow.GetDialogViewModel().OnDialogClosed();

                    if (dialogWindow.Result == null)
                        dialogWindow.Result = new DialogResult();

                    callback?.Invoke(dialogWindow.Result);

                    dialogWindow.DataContext = null;
                    dialogWindow.Content = null;
                };
            dialogWindow.Closed += closedHandler;
        }

        protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            var windowStyle = Dialog.GetWindowStyle(dialogContent);
            if (windowStyle != null)
            {
                window.Style = windowStyle;
            }

            window.Content = dialogContent;
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data context

            window.Owner ??= Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        }
    }

    internal static class IDialogWindowExtensions
    {
        /// <summary>
        /// Get the <see cref="IDialogAware"/> ViewModel from a <see cref="IDialogWindow"/>.
        /// </summary>
        /// <param name="dialogWindow"><see cref="IDialogWindow"/> to get ViewModel from.</param>
        /// <returns>ViewModel as a <see cref="IDialogAware"/>.</returns>
        internal static IDialogAware GetDialogViewModel(this IDialogWindow dialogWindow)
        {
            return (IDialogAware)dialogWindow.DataContext;
        }
    }
}
