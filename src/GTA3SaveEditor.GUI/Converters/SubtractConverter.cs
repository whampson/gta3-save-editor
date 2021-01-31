using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class SubtractConverter : IMultiValueConverter
    {
        public bool ClampToZero { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double[] x = values.Cast<double>().ToArray();
            double r = x[0];
            for (int i = 1; i < x.Length; i++)
            {
                r -= x[i];
            }
            
            return (r < 0 && ClampToZero) ? 0 : r;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported for this converter.");
        }
    }
}
