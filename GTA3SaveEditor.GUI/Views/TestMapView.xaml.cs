using GTA3SaveEditor.GUI.ViewModels;
using System.Windows;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for TestMapView.xaml
    /// </summary>
    public partial class TestMapView : TabPageBase<TestMap>
    {
        public TestMapView()
        {
            InitializeComponent();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ViewModel.ContextMenuOffset = m_map.MouseMapCoords;
            ViewModel.ContextMenuCoords = m_map.MouseWorldCoords;

            ViewModel.IsBlipSelected = m_map.GetBlip(m_map.MouseWorldCoords, out MapBlip selected);
            ViewModel.SelectedBlip = selected;
        }

        private void m_map_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.IsMouseOverBlip = m_map.GetBlip(m_map.MouseWorldCoords, out var _);
        }
    }
}
