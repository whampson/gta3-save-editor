using GTA3SaveEditor.GUI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public static readonly DependencyProperty BlipsProperty = DependencyProperty.Register(
            nameof(Blips), typeof(ObservableCollection<MapBlip>), typeof(MapView),
            new FrameworkPropertyMetadata(
                new ObservableCollection<MapBlip>(),
                new PropertyChangedCallback(BlipsPropertyChanged)));

        public static readonly DependencyProperty MouseWorldCoordsProperty = DependencyProperty.Register(
            nameof(MouseWorldCoords), typeof(Point), typeof(MapView));

        public static readonly DependencyProperty MouseMapCoordsProperty = DependencyProperty.Register(
            nameof(MouseMapCoords), typeof(Point), typeof(MapView));

        public ObservableCollection<MapBlip> Blips
        {
            get { return (ObservableCollection<MapBlip>) GetValue(BlipsProperty); }
            set { SetValue(BlipsProperty, value); }
        }

        public Point MouseWorldCoords
        {
            get { return (Point) GetValue(MouseWorldCoordsProperty); }
            set { SetValue(MouseWorldCoordsProperty, value); }
        }

        public Point MouseMapCoords
        {
            get { return (Point) GetValue(MouseMapCoordsProperty); }
            set { SetValue(MouseMapCoordsProperty, value); }
        }

        public MapView()
        {
            InitializeComponent();
        }

        public bool GetBlip(Point coords, out MapBlip blip)
        {
            blip = Blips.Where(b =>
                    (coords.X <= (b.WorldCoords.X + (Math.Abs(b.Width  / m_map.Scale.X) * b.Scale) / 2)) &&
                    (coords.X >= (b.WorldCoords.X - (Math.Abs(b.Width  / m_map.Scale.X) * b.Scale) / 2)) &&
                    (coords.Y <= (b.WorldCoords.Y + (Math.Abs(b.Height / m_map.Scale.Y) * b.Scale) / 2)) &&
                    (coords.Y >= (b.WorldCoords.Y - (Math.Abs(b.Height / m_map.Scale.Y) * b.Scale) / 2)))
                .FirstOrDefault();

            return blip != null;
        }

        public void ScaleBlipsUp()
        {
            // TOOD: max
            for (int i = 0; i < Blips.Count; i++)
            {
                var blip = Blips[i];
                blip.Scale *= 1.1;
                Blips[i] = blip;
            }
        }

        public void ScaleBlipsDown()
        {
            // TODO: min
            for (int i = 0; i < Blips.Count; i++)
            {
                var blip = Blips[i];
                blip.Scale /= 1.1;
                Blips[i] = blip;
            }
        }

        private void DrawBlip(MapBlip blip)
        {
            Matrix m = Matrix.Identity;
            UIElement e = blip.UIElement;
            Point p = m_map.WorldToPixel(blip.WorldCoords);

            double centerX = blip.Width * blip.Scale / 2;
            double centerY = blip.Height * blip.Scale / 2;

            m.RotateAt(blip.Angle, centerX, centerY);
            m.OffsetX = p.X - centerX;
            m.OffsetY = p.Y - centerY;
            m.ScalePrepend(blip.Scale, blip.Scale);
            e.RenderTransform = new MatrixTransform(m);

            // TOOD: test this
            if (blip.ZIndex > 0) Canvas.SetZIndex(e, blip.ZIndex);

            //m_map.AddOverlay(e);
        }

        private void EraseBlip(MapBlip blip)
        {
            //m_map.RemoveOverlay(blip.UIElement);
        }

        private void DrawAllBlips()
        {
            //foreach (var blip in Blips) DrawBlip(blip);
        }

        private void EraseAllBlips()
        {
            //m_map.RemoveAllOverlays();
        }

        private static void BlipsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MapView view)
            {
                var collectionChangedAction = new NotifyCollectionChangedEventHandler((sender, e) =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems) view.DrawBlip(item as MapBlip);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems) view.EraseBlip(item as MapBlip);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            foreach (var item in e.OldItems) view.EraseBlip(item as MapBlip);
                            foreach (var item in e.NewItems) view.DrawBlip(item as MapBlip);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            view.EraseAllBlips();
                            foreach (var item in e.NewItems) view.DrawBlip(item as MapBlip);
                            break;
                    }
                });

                if (e.OldValue != null)
                {
                    var oldCollection = (INotifyCollectionChanged) e.OldValue;
                    oldCollection.CollectionChanged -= collectionChangedAction;
                    view.EraseAllBlips();
                }
                if (e.NewValue != null)
                {
                    var newCollection = (INotifyCollectionChanged) e.NewValue;
                    newCollection.CollectionChanged += collectionChangedAction;
                    view.DrawAllBlips();
                }
            }
        }

        public ICommand ScaleUpBlipsCommand => new RelayCommand(() => ScaleBlipsUp());
        public ICommand ScaleDownBlipsCommand => new RelayCommand(() => ScaleBlipsDown());
    }
}
