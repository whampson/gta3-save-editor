using System;
using System.Globalization;
using System.Windows;
using GTASaveData;

namespace GTA3SaveEditor.GUI.Converters
{
    public class DateTimeConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime theDate;

            if (value is SystemTime st)
            {
                theDate = (DateTime) st;
            }
            else if (value is Date d)
            {
                theDate = (DateTime) d;
            }
            else if (value is DateTime dt)
            {
                theDate = dt;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }

            return theDate.ToString(parameter as string ?? "");
        }
    }
}
