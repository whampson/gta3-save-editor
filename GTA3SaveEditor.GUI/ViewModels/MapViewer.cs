using System.Windows;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class MapViewer : TabPageViewModelBase
    {
        private double m_zoomLevel;
        private Point m_coords1;
        private Point m_coords2;

        public double ZoomLevel
        {
            get { return m_zoomLevel; }
            set { m_zoomLevel = value; OnPropertyChanged(); }
        }

        public Point Coords1
        {
            get { return m_coords1; }
            set { m_coords1 = value; OnPropertyChanged(); }
        }

        public Point Coords2
        {
            get { return m_coords2; }
            set { m_coords2 = value; OnPropertyChanged(); }
        }

        public MapViewer(Main mainViewModel)
            : base("Map Viewer", TabPageVisibility.Always, mainViewModel)
        { }
    }
}
