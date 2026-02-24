using System;

using Prism.Services.Dialogs;

namespace Aksl.Toolkit.Services
{
    public static class DialogServiceExtensions
    {
        public static void Alert(this IDialogService dialogService, string message, string title, string okText, Action<IDialogResult> callBack) => 
            dialogService.ShowDialog(nameof(Dialogs.ConfirmView), new DialogParameters { { "IsConfirm", false }, { "Message", message }, { "Title", title }, { "OkText", okText }, }, callBack);

        public static void Confirm(this IDialogService dialogService, string message, string title, string okText, string cancelText, Action<IDialogResult> callBack) => 
            dialogService.ShowDialog(nameof(Dialogs.ConfirmView), new DialogParameters { { "IsConfirm", true }, { "Message", message }, { "Title", title }, { "OkText", "确定" }, { "CancelText", "取消" } }, callBack);
    }
}
