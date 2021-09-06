using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class AddConverter : MultiConverterBase
    {
        public bool Clamp { get; set; }
        public double ClampValue { get; set; }
        
        // TODO: support other types?
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double[] x = values.Cast<double>().ToArray();
            double r = x[0];
            for (int i = 1; i < x.Length; i++)
            {
                r += x[i];
            }

            return (Clamp && r > ClampValue) ? ClampValue : r;
        }
    }
}
