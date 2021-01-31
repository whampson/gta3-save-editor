using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.GUI.Types;

namespace GTA3SaveEditor.GUI.Converters
{
    public class NumberFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 0 && values[0] is int x)
            {
                if (values.Length > 1 && values[1] is NumberFormat numFormat)
                {
                    return numFormat switch
                    {
                        NumberFormat.Int => x.ToString(),
                        NumberFormat.Float => BitConverter.Int32BitsToSingle(x).ToString(),
                        NumberFormat.Hex => x.ToString("X8"),
                        _ => x.ToString(),
                    };
                }
                else
                {
                    return x.ToString();
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter.");
        }
    }
}
