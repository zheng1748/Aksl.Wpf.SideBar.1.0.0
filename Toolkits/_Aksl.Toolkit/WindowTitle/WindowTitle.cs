#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Windows;
using System.Windows.Controls;

namespace Aksl.Toolkit.UI.Controls
{
    public class WindowTitle : Control
    {
        #region Prefix Property
        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        public static readonly DependencyProperty PrefixProperty =
                      DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(WindowTitle), new PropertyMetadata(null, OnTitleChanged));
        #endregion

        #region Title Property
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
                      DependencyProperty.Register(nameof(Title), typeof(string), typeof(WindowTitle), new PropertyMetadata(null, OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowTitle control)
            {
                Application.Current.MainWindow.Title = $"{control.Prefix}{control.Title}".Trim();
            }
        }
        #endregion
    }
}
