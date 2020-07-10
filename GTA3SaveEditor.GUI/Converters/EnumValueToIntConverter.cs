using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class EnumValueToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object retval = DependencyProperty.UnsetValue;
            if (parameter is Type t)
            {
                retval = Enum.ToObject(t, (int) value);
            }

            return retval;
        }
    }
}
