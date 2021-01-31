using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class ListViewItemIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ListViewItem item)
            {
                ListView view = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                int index = view.ItemContainerGenerator.IndexFromContainer(item);
                return index.ToString();
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported for this converter.");
        }
    }
}
