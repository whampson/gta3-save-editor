using System;
using System.Globalization;
using System.Windows;
using GTA3SaveEditor.Core.Game;

namespace GTA3SaveEditor.GUI.Converters
{
    public class GxtStringConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string key)
            {
                if (key.Length > 0 && key[0] == '\uFFFF')
                {
                    key = key.Substring(1);
                }
                return GTA3.GetGxtString(key);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
