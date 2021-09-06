using System.Windows.Controls;

namespace GTA3SaveEditor.GUI.Tabs
{
    /// <summary>
    /// Interaction logic for PickupsTab.xaml
    /// </summary>
    public partial class PickupsTab : TabPage<PickupsVM>
    {
        public PickupsTab()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
