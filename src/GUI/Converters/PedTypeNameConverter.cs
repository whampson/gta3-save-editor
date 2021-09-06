using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Converters
{
    public class PedTypeNameConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PedTypeId pedType)
            {
                int id = (int) pedType;
                return App.FindString($"PedType{id:00}Name");
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
