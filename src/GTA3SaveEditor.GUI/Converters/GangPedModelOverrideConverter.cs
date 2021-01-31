using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.Core;

namespace GTA3SaveEditor.GUI.Converters
{
    [ValueConversion(typeof(byte), typeof(GangPedModelState))]
    public class GangPedModelOverrideConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is sbyte pedModelOverride)
            {
                return pedModelOverride switch
                {
                    0 => GangPedModelState.Model1,
                    1 => GangPedModelState.Model2,
                    _ => GangPedModelState.Both
                };
            }

            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GangPedModelState state)
            {
                sbyte ret = state switch
                {
                    GangPedModelState.Model1 => 0,
                    GangPedModelState.Model2 => 1,
                    _ => -1
                };
                return ret;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
