using System.Linq;
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
            var selected = ViewModel.ThreadInfo.Where(t => t.IsSelected).ToList();
            
            ViewModel.LocalIndex = -1;
            ViewModel.StackIndex = -1;

            if (selected.Count == 1)
            {
                ViewModel.MultipleThreadsSelected = false;
                ViewModel.SelectedThread = selected.First();
                if (sender is ListBox listBox)
                    listBox.ScrollIntoView(ViewModel.SelectedThread);
            }
            else
            {
                ViewModel.MultipleThreadsSelected = (selected.Count != 0);
                ViewModel.SelectedThread = null;
            }
        }

        private void GlobalListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ViewModel.GlobalInfo.Where(g => g.IsSelected).ToList();
            if (selected.Count == 1)
            {
                ViewModel.MultipleGlobalsSelected = false;
                ViewModel.SelectedGlobal = selected.First();
                ViewModel.ReadSelectedGlobalValue();
            }
            else
            {
                ViewModel.MultipleGlobalsSelected = (selected.Count != 0);
                ViewModel.SelectedGlobal = null;
            }
        }

        private void GlobalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && e.OldValue != null)
            {
                ViewModel.WriteGlobalValue();
            }
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
