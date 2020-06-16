using System;
using System.Globalization;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public bool Default { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
            {
                return Default;
            }

            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter.");
        }
    }
}
