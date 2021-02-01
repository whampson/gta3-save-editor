using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GTA3SaveEditor.GUI.Tabs
{
    public partial class ScriptsTab : TabPage<ScriptsTabVM>
    {
        public ScriptsTab()
        {
            InitializeComponent();
        }

        private void GlobalsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = $"${e.Row.GetIndex()}";
        }

        private void LocalsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = $"@{e.Row.GetIndex()}";
        }

        private void StackDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex().ToString();
        }

        private void ThreadListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.LocalIndex = -1;
            ViewModel.StackIndex = -1;

            if (ViewModel.Thread != null && sender is ListBox listBox)
            {
                listBox.ScrollIntoView(ViewModel.Thread);
            }
        }

        private void GlobalListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ReadGlobalValue();
        }

        private void GlobalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.WriteGlobalValue();
        }

        private void LocalListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ReadLocalValue();
        }

        private void LocalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.WriteLocalValue();
        }

        private void StackListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ReadStackValue();
        }

        private void StackUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.WriteStackValue();
        }
    }
}
