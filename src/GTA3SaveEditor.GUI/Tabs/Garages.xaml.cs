using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Tabs
{
    /// <summary>
    /// Interaction logic for Garages.xaml
    /// </summary>
    public partial class GaragesTab : TabPage<GaragesVM>
    {
        public GaragesTab()
        {
            InitializeComponent();
        }

        private void SafehouseComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ViewModel.UpdateStoredCarList();
        }
    }
}
