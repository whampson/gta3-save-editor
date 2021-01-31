using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class FloatBitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int x)
            {
                return BitConverter.Int32BitsToSingle(x);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float f)
            {
                return BitConverter.SingleToInt32Bits(f);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
