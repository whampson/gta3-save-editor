using System;
using System.Globalization;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class GlitchStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? hasBeenFixed = value as bool?;
            if (hasBeenFixed == null)
            {
                return "Not Detected";
            }

            return (hasBeenFixed == true) ? "Fixed" : "Detected";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter.");
        }
    }
}
