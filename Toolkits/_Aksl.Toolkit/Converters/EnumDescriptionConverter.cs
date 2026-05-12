using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Aksl.Toolkit.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Enum)value is Enum e)
            {
                string description = GetEnumDescription(e);
                return description;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        private string GetEnumDescription(Enum e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray[0] is DescriptionAttribute attrib)
            {
                return attrib.Description;
            }

            return e.ToString();

            //if (attribArray.Length == 0)
            //{
            //    return e.ToString();
            //}
            //else
            //{
            //    DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
            //    return attrib.Description;
            //}
        }
    }
}


