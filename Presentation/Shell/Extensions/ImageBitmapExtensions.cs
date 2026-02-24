/*
 * Copyright (c) 2021 All Rights Reserved.
 * Description：DpiHelper
 * Author： Chance_写代码的厨子
 * Create Time：2021-05-24 17:41:50
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Aksl.Windows.Extensions
{
    public static class ImageBitmapExtensions
    {

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            IntPtr intPtr = bitmap.GetHbitmap();
            try
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return imageSource;
            }
            finally
            {
                InteropMethods.DeleteObject(intPtr);
            }
        }
    }

    public sealed class InteropMethods
    {

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport(InteropValues.ExternDll.Gdi32, EntryPoint = nameof(DeleteObject), CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IntDeleteObject(IntPtr hObject);

        [SecurityCritical]
        public static bool DeleteObject(IntPtr hObject)
        {
            var result = IntDeleteObject(hObject);
            return result;
        }
    }

    public sealed class InteropValues
    {
        #region Const

        public static class ExternDll
        {
            public const string User32 = "user32.dll",
                                Gdi32 = "gdi32.dll",
                                GdiPlus = "gdiplus.dll",
                                Kernel32 = "kernel32.dll",
                                Shell32 = "shell32.dll",
                                MsImg = "msimg32.dll",
                                NTdll = "ntdll.dll",
                                Dwmapi = "dwmapi.dll",
                                Ole32 = "ole32.dll";
        }
        #endregion
    }
}
