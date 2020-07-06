﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class LogicalOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;

            foreach (var item in values)
            {
                if (!(item is bool b))
                {
                    return false;
                }

                result |= b;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported for this converter.");
        }
    }
}
