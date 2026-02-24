using System;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Aksl.Toolkit.Dialogs
{
    public class ConfirmViewModel : BindableBase, IDialogAware
    {
        #region Constructors
        public ConfirmViewModel()
        {
            CreateCommand();
        }
        #endregion

        #region Properties
        private bool _isConfirm;
        public bool IsConfirm
        {
            get => _isConfirm;
            set => SetProperty<bool>(ref _isConfirm, value);
        }

        private string _okText = "OK";
        public string OkText
        {
            get => _okText;
            set => SetProperty(ref _okText, value);
        }

        private string _cancelText = "Cancel";
        public string CancelText
        {
            get => _cancelText;
            set => SetProperty(ref _cancelText, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _title = "Notification";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        #endregion

        #region Create Command
        public DelegateCommand<string> OKCommand { get; private set; }

        public DelegateCommand<string> CancelCommand { get; private set; }

        private void CreateCommand()
        {
            OKCommand = new DelegateCommand<string>(ExecuteOKOrCancelCommand,
            (parameter) =>
            {
                return true;
            });

            CancelCommand = new DelegateCommand<string>(ExecuteOKOrCancelCommand,
            (parameter) =>
            {
                return true;
            });
        }

        private void ExecuteOKOrCancelCommand(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.OK;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.Cancel;
            }

            RequestClose?.Invoke(new DialogResult(result));
        }
        #endregion

        #region IDialogAware
        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("Message");
            Title = parameters.GetValue<string>("Title") ?? "Notification";
            OkText = parameters.GetValue<string>("OkText") ?? "OK";
            CancelText = parameters.GetValue<string>("CancelText") ?? "Cancel";
            IsConfirm = parameters.GetValue<bool?>("IsConfirm") ?? true;
        }
        #endregion
    }
}
