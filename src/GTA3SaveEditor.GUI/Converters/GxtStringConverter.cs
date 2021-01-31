using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.Core;

namespace GTA3SaveEditor.GUI.Converters
{
    public class GxtStringConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string key)
            {
                return GTA3.GetGxtString(key);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
