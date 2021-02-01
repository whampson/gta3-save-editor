using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class ScriptSymbolConverter : MultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 &&
                values[0] is Dictionary<int, string> symbols &&
                values[1] is int index)
            {
                if (!symbols.TryGetValue(index, out string symbol))
                {
                    return string.Empty;
                }

                return symbol;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
