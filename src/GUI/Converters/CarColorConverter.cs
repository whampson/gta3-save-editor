using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.GUI.Extensions;

namespace GTA3SaveEditor.GUI.Converters
{
    [ValueConversion(typeof(byte), typeof(System.Windows.Media.Color))]
    public class CarColorConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte c)
            {
                var colorInfo = GTA3.CarColors.ElementAtOrDefault(c);
                if (colorInfo == null)
                {
                    return Colors.Black;
                }
                return colorInfo.Color.ToMediaColor();
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                var c = GTA3.CarColors.FirstOrDefault(x => x.Color.ToMediaColor() == color);
                if (c == default)
                {
                    return (byte) 0;
                }

                return (byte) GTA3.CarColors.ToList().IndexOf(c);
            }

            throw new NotSupportedException($"Cannot convert '{value}' to type {targetType}.");
        }
    }
}
