using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GTA3SaveEditor.GUI.Converters
{
    public class ListViewItemIndexConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ListViewItem item)
            {
                ListView view = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                int index = view.ItemContainerGenerator.IndexFromContainer(item);
                return index;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
