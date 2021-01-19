﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.Core;

namespace GTA3SaveEditor.GUI.Converters
{
    [ValueConversion(typeof(VehicleModel), typeof(string))]
    public class VehicleModelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VehicleModel v)
            {
                return v.ModelName switch
                {
                    "" => "(none)",
                    "corpse" => "Manana (corpse)",
                    "chopper" => "Chopper",
                    "escape" => "Escape",
                    _ => GTA3.GetGxtString(v.GameName),
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
