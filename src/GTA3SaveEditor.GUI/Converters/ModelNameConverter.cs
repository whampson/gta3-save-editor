using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GTA3SaveEditor.Core;

namespace GTA3SaveEditor.GUI.Converters
{
    public class ModelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short index)
            {
                IdeObject ideObj = SaveEditor.GetIdeObject(index);
                if (ideObj != null)
                {
                    return ideObj.ModelName;
                }

                if (index == 0)
                {
                    return string.Empty;
                }

                return index.ToString();
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return (short) 0;
            }

            if (value is string model)
            {
                IdeObject ideObj = SaveEditor.GetIdeObject(model);
                if (ideObj != null)
                {
                    return ideObj.Id;
                }

                if (short.TryParse(model, out short index))
                {
                    return index;
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
