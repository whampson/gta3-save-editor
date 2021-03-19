using System.Windows;
using System.Windows.Controls;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Tabs
{
    /// <summary>
    /// Interaction logic for GangsTab.xaml
    /// </summary>
    public partial class GangsTab : TabPage<GangsVM>
    {
        private bool m_suppressDensityChanged;

        public GangsTab()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = ViewModel.GangIndex;
            ViewModel.Gang = (i == -1) ? null : ViewModel.TheSave.Gangs[i];
            ViewModel.Update();
        }

        private void ZoneListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_suppressDensityChanged = true;
            ViewModel.ReadDensity();
            m_suppressDensityChanged = false;
        }

        private void PedDensityDay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (m_suppressDensityChanged) return;
            ViewModel.WritePedDensityDay();
        }

        private void PedDensityNight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (m_suppressDensityChanged) return;
            ViewModel.WritePedDensityNight();
        }

        //private void CarDensityDay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    if (m_suppressDensityChanged) return;
        //    ViewModel.WriteCarDensityDay();
        //}

        //private void CarDensityNight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    if (m_suppressDensityChanged) return;
        //    ViewModel.WriteCarDensityNight();
        //}
    }
}
