using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace GTA3SaveEditor.GUI.Tabs
{
    public partial class ScriptsTab : TabPage<ScriptsVM>
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

        private bool m_suppressGlobalValueChanged;

        private void GlobalListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ViewModel.GlobalInfo.Where(g => g.IsSelected).ToList();
            if (selected.Count == 1)
            {
                m_suppressGlobalValueChanged = true;
                int index = selected.First().Index;
                int? value = ViewModel.GlobalInfo[index].Value;
                ViewModel.SelectedGlobalValue = value;
                ViewModel.MultipleGlobalsSelected = false;
                m_suppressGlobalValueChanged = false;
            }
            else
            {
                ViewModel.MultipleGlobalsSelected = (selected.Count != 0);
                ViewModel.SelectedGlobalValue = null;
            }
        }

        private void GlobalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && e.OldValue != null && !m_suppressGlobalValueChanged)
            {
                int index = ViewModel.SelectedGlobalIndex;
                if (index < 0) return;

                bool changed = false;
                object iOld = ViewModel.TheSave.Script.GetGlobalVariable(index);
                object fOld = ViewModel.TheSave.Script.GetGlobalVariableFloat(index);
                object iNew = e.NewValue as int?;
                object fNew = e.NewValue as float?;
                var numFmt = ViewModel.NumberFormat;

                if (sender is SingleUpDown
                    && numFmt == Types.NumberFormat.Float
                    && !fNew.Equals(fOld))
                {
                    ViewModel.TheSave.Script.SetGlobalVariable(index, (float) e.NewValue);
                    changed = true;
                }
                else if (sender is IntegerUpDown
                    && (numFmt == Types.NumberFormat.Hex || numFmt == Types.NumberFormat.Int)
                    && !iNew.Equals(iOld))
                {
                    ViewModel.TheSave.Script.SetGlobalVariable(index, (int) e.NewValue);
                    changed = true;
                }

                if (changed)
                {
                    int v = ViewModel.TheSave.Script.GetGlobalVariable(index);
                    ViewModel.GlobalInfo[index].Value = v;
                    ViewModel.MarkDirty($"{nameof(ViewModel.TheSave.Script.GlobalVariables)}[{index}]", v);
                }
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
