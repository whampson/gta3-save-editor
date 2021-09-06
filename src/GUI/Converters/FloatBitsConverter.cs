using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.Core.Extensions;

namespace GTA3SaveEditor.GUI.Converters
{
    public class FloatBitsConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int || value is uint)
            {
                int int32Value = (int) System.Convert.ChangeType(value, typeof(int));
                return BitConverter.Int32BitsToSingle(int32Value);
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float f)
            {
                Type t = Nullable.GetUnderlyingType(targetType) ?? targetType;
                if (t == typeof(int) || t == typeof(uint))
                {
                    int bits = BitConverter.SingleToInt32Bits(f);
                    return System.Convert.ChangeType(bits, t);
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
