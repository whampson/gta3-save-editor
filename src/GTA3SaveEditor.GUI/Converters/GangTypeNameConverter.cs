using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Converters
{
    [ValueConversion(typeof(GangType), typeof(string))]
    public class GangTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GangType || value is int)
            {
                return (GangType) value switch
                {
                    GangType.Mafia => "Mafia",
                    GangType.Triads => "Triads",
                    GangType.Diablos => "Diablos",
                    GangType.Yakuza => "Yakuza",
                    GangType.Yardies => "Uptown Yardies",
                    GangType.Cartel => "Colombian Cartel",
                    GangType.Hoods => "Southside Hoods",
                    GangType.Gang8 => "GANG8",
                    GangType.Gang9 => "GANG9",
                    _ => DependencyProperty.UnsetValue
                };
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter.");
        }
    }
}
