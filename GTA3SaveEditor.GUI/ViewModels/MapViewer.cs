using System.Windows;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class MapViewer : TabPageViewModelBase
    {
        private double m_zoomLevel;
        private Point m_panOffset;
        private Point m_mouseOverOffset;
        private Point m_mouseOverCoords;

        public double ZoomLevel
        {
            get { return m_zoomLevel; }
            set { m_zoomLevel = value; OnPropertyChanged(); }
        }

        public Point PanOffset
        {
            get { return m_panOffset; }
            set { m_panOffset = value; OnPropertyChanged(); }
        }

        public Point ContextMenuOffset
        {
            get { return m_mouseOverOffset; }
            set { m_mouseOverOffset = value; OnPropertyChanged(); }
        }

        public Point MouseOverCoords
        {
            get { return m_mouseOverCoords; }
            set { m_mouseOverCoords = value; OnPropertyChanged(); }
        }

        public MapViewer(Main mainViewModel)
            : base("Map Viewer", TabPageVisibility.Always, mainViewModel)
        { }

        public void PanHere()
        {
            Point p = ContextMenuOffset;
            p.X = -p.X;
            p.Y = -p.Y;

            PanOffset = p;
        }

        public void ZoomIn()
        {
            ZoomLevel *= 1.1;
        }

        public void ZoomOut()
        {
            ZoomLevel *= 1 / 1.1;
        }

        public ICommand PanHereCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => PanHere()
                );
            }
        }

        public ICommand ZoomInCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => ZoomIn()
                );
            }
        }

        public ICommand ZoomOutCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => ZoomOut()
                );
            }
        }
    }
}
