using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aksl.Toolkit.Controls
{
    public class PackIcon : Control
    {
        #region Members 
        private static readonly Lazy<IDictionary<PackIconKind, string>> _dataLookup
                                                                            = new(PackIconDataFactory.Create);
        #endregion

        #region Constructors
        static PackIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
        }
        #endregion

        #region Kind Property
        public PackIconKind Kind
        {
            get => (PackIconKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public static readonly DependencyProperty KindProperty
                     = DependencyProperty.Register(nameof(Kind), typeof(PackIconKind), typeof(PackIcon), new PropertyMetadata(defaultValue: default(PackIconKind), propertyChangedCallback: OnKindPropertyChanged));

        private static void OnKindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PackIcon packIcon)
            {
                if (e.NewValue is PackIconKind newValue && e.OldValue is PackIconKind oldValue)
                {
                    if (newValue != oldValue)
                    {
                        packIcon.UpdateData();
                    }
                }
            }
        }
        #endregion

        #region Data Property
        private static readonly DependencyPropertyKey DataPropertyKey
                          = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(defaultValue: null, propertyChangedCallback: null));

        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the icon path data for the current <see cref="Kind"/>.
        /// </summary>
        [TypeConverter(typeof(GeometryConverter))]
        public string? Data
        {
            get => (string?)GetValue(DataProperty);
            private set => SetValue(DataPropertyKey, value);
        }
        #endregion

        #region ApplyTemplate Method
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateData();
        }

        private void UpdateData()
        {
            string? data = null;
            _dataLookup.Value?.TryGetValue(Kind, out data);
            Data = data;
        }
        #endregion
    }
}