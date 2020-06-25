using GTA3SaveEditor.GUI.ViewModels;
using System.Windows.Forms;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : TabPageBase<MapViewer>
    {
        public MapView()
        {
            InitializeComponent();
        }

        private void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.ContextMenuOffset = m_map.MouseOverOffset;
            m_map.DrawBlip(m_map.MouseOverOffset);
        }
    }
}
