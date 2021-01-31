using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.Core;

namespace GTA3SaveEditor.GUI.Converters
{
    [ValueConversion(typeof(int), typeof(VehicleModel))]
    public class VehicleModelConverter : ConverterBase
    {
        public int NoneValue { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                if (index == NoneValue)
                {
                    return GTA3.GetVehicle(0);
                }

                VehicleModel v = GTA3.GetVehicle(index);
                if (v != null)
                {
                    return v;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return NoneValue;
            }

            if (value is VehicleModel v)
            {
                return v.Id;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
