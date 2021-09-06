using System;
using System.Globalization;
using System.Windows;

namespace GTA3SaveEditor.GUI.Converters
{
    public class EnumFlagsConverter : ConverterBase
    {
        private Enum m_target;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum e && parameter is Enum flag)
            {
                m_target = e;
                return e.HasFlag(flag);
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool v && parameter is Enum flag)
            {
                int e = EnumToInt(m_target);
                int f = EnumToInt(flag);

                int result = (v) ? (e | f) : (e & ~f);

                Enum retval = (Enum) Enum.Parse(targetType, result.ToString());
                return retval;
            }
            throw new NotSupportedException($"Cannot convert '{value}' to type {targetType}.");
        }

        private static int EnumToInt(Enum e) => (int) (object) e;
    }
}
