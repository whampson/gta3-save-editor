using GTA3SaveEditor.GUI.Helpers;
using GTA3SaveEditor.GUI.Views;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GTA3SaveEditor.GUI.Converters
{
    public class RadarBlipColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var colorId = values[0] as int?;
            var isBright = values[1] as bool?;
            var isVisible = values[2] as bool?;
            var hasSprite = values[3] as bool?;

            if (colorId != null && isBright != null && isVisible == true && hasSprite == false)
            {
                return MapHelper.GetBlipColor((int) colorId, (bool) isBright);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object colorId = DependencyProperty.UnsetValue;
            object isBright = DependencyProperty.UnsetValue;

            if (value is Color color)
            {
                colorId = 0;
                isBright = false;

                var colors = MapHelper.StandardBlipColors.Select(x => x.Color).ToList();
                int index = colors.IndexOf(color);

                if (index > -1)
                {
                    if ((index % 2) == 1)
                    {
                        isBright = true;
                    }
                    colorId = index / 2;
                }
                else
                {
                    colorId = (color.R << 24) | (color.G << 16) | (color.B << 8);
                }
            }

            return new object[] { colorId, isBright };
        }
    }
}
