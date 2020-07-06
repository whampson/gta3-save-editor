using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.ViewModels;
using GTASaveData.GTA3;
using GTASaveData.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfEssentials.Win32;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for RadarView.xaml
    /// </summary>
    public partial class RadarView : TabPageBase<Radar>
    {
        public const int NumStandardColorTypes = 7;

        public static readonly DependencyProperty MapZoomProperty = DependencyProperty.Register(
            nameof(MapZoom), typeof(double), typeof(RadarView));

        public static readonly DependencyProperty MapPanProperty = DependencyProperty.Register(
            nameof(MapPan), typeof(Point), typeof(RadarView));

        public static readonly DependencyProperty MouseCursorProperty = DependencyProperty.Register(
            nameof(MouseCursor), typeof(Cursor), typeof(RadarView));

        public static readonly DependencyProperty MouseOverMapCoordsProperty = DependencyProperty.Register(
            nameof(MouseOverMapCoords), typeof(Point), typeof(RadarView));

        public static readonly DependencyProperty MouseOverWorldCoordsProperty = DependencyProperty.Register(
            nameof(MouseOverWorldCoords), typeof(Point), typeof(RadarView));

        public static readonly DependencyProperty MouseOverBlipProperty = DependencyProperty.Register(
            nameof(MouseOverBlip), typeof(RadarBlip), typeof(RadarView));

        public static readonly DependencyProperty MouseClickWorldCoordsProperty = DependencyProperty.Register(
            nameof(MouseClickWorldCoords), typeof(Point), typeof(RadarView));

        public static readonly DependencyProperty BlipsProperty = DependencyProperty.Register(
            nameof(Blips), typeof(ObservableCollection<UIElement>), typeof(RadarView),
            new PropertyMetadata(
                new ObservableCollection<UIElement>()));

        public double MapZoom
        {
            get { return (double) GetValue(MapZoomProperty); }
            set { SetValue(MapZoomProperty, value); }
        }

        public Point MapPan
        {
            get { return (Point) GetValue(MapPanProperty); }
            set { SetValue(MapPanProperty, value); }
        }

        public Cursor MouseCursor
        {
            get { return (Cursor) GetValue(MouseCursorProperty); }
            set { SetValue(MouseCursorProperty, value); }
        }

        public Point MouseOverMapCoords
        {
            get { return (Point) GetValue(MouseOverMapCoordsProperty); }
            set { SetValue(MouseOverMapCoordsProperty, value); }
        }

        public Point MouseOverWorldCoords
        {
            get { return (Point) GetValue(MouseOverWorldCoordsProperty); }
            set { SetValue(MouseOverWorldCoordsProperty, value); }
        }

        public RadarBlip MouseOverBlip
        {
            get { return (RadarBlip) GetValue(MouseOverBlipProperty); }
            set { SetValue(MouseOverBlipProperty, value); }
        }

        public Point MouseClickWorldCoords
        {
            get { return (Point) GetValue(MouseClickWorldCoordsProperty); }
            set { SetValue(MouseClickWorldCoordsProperty, value); }
        }

        public ObservableCollection<UIElement> Blips
        {
            get { return (ObservableCollection<UIElement>) GetValue(BlipsProperty); }
            set { SetValue(BlipsProperty, value); }
        }

        private readonly Dictionary<RadarBlip, UIElement> m_blipUIElementMap;
        private bool m_suppressContextMenuOnce;
        private bool m_isContextMenuVisible;
        private bool m_wasContextMenuOpenedOnBlip;
        private RadarBlip m_grabbedBlip;

        public RadarView()
        {
            m_blipUIElementMap = new Dictionary<RadarBlip, UIElement>();
            InitializeComponent();

            MouseCursor = Cursors.Cross;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ViewModel.BlipUpdate += ViewModel_BlipUpdate;
            ViewModel.DrawAllBlips();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            ViewModel.BlipUpdate -= ViewModel_BlipUpdate;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            m_map.Reset();
        }

        private UIElement MakeBlip(RadarBlip blip)
        {
            const double Size = 2;

            SolidColorBrush brush = new SolidColorBrush
            {
                Color = GetBlipColor(blip.Color, blip.IsBright)
            };
            Rectangle rect = new Rectangle
            {
                Fill = brush,
                StrokeThickness = 0.5,
                Stroke = Brushes.Black,
                Width = Size * blip.Scale,
                Height = Size * blip.Scale
            };

            Point p = m_map.WorldToPixel(new Point(blip.RadarPosition.X, blip.RadarPosition.Y));

            Matrix m = Matrix.Identity;
            m.OffsetX = p.X - (rect.Width / 2);
            m.OffsetY = p.Y - (rect.Height / 2);

            rect.RenderTransform = new MatrixTransform(m);
            return rect;
        }

        private UIElement MakeSpriteBlip(RadarBlip blip, string uri)
        {
            const int Size = 16;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(uri);
            bmp.DecodePixelWidth = Size;
            bmp.EndInit();

            Image img = new Image()
            {
                Source = bmp,
                Width = bmp.Width,
                Height = bmp.Height,
            };

            Point p = m_map.WorldToPixel(new Point(blip.RadarPosition.X, blip.RadarPosition.Y));

            Matrix m = Matrix.Identity;
            m.OffsetX = p.X - (Size / 2);
            m.OffsetY = p.Y - (Size / 2);

            img.RenderTransform = new MatrixTransform(m);
            return img;
        }

        private void RemoveAllBlips()
        {
            Blips.Clear();
            m_blipUIElementMap.Clear();
        }

        private void UpdateBlip(RadarBlip blip)
        {
            if (m_blipUIElementMap.TryGetValue(blip, out UIElement oldElement))
            {
                Blips.Remove(oldElement);
                m_blipUIElementMap.Remove(blip);
            }

            if (blip.IsVisible)
            {
                UIElement newElement = (blip.Sprite == RadarBlipSprite.None)
                    ? MakeBlip(blip)
                    : MakeSpriteBlip(blip, SpriteURIs[blip.Sprite]);

                Blips.Add(newElement);
                m_blipUIElementMap[blip] = newElement;
            }
        }

        private void LocateBlip(RadarBlip blip)
        {
            MapPan = m_map.WorldToMap(new Point(-blip.RadarPosition.X, -blip.RadarPosition.Y));
            MapZoom = 4.5;
        }

        private void GrabBlip(RadarBlip blip)
        {
            m_grabbedBlip = blip;
            UpdateCursor();
        }

        private void DropBlip(Point loc)
        {
            ViewModel.MoveBlip(m_grabbedBlip, loc);
            m_grabbedBlip = null;
            UpdateCursor();
        }

        private void CancelMove()
        {
            m_grabbedBlip = null;
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            if (m_grabbedBlip != null)
            {
                MouseCursor = Cursors.ScrollAll;
            }
            else if (MouseOverBlip != null)
            {
                MouseCursor = Cursors.Hand;
            }
            else
            {
                MouseCursor = Cursors.Cross;
            }
        }

        public ICommand NewBlipCommand => new RelayCommand
        (
            () => ViewModel.CreateBlip(MouseClickWorldCoords),
            () => !m_wasContextMenuOpenedOnBlip && ViewModel.NextAvailableSlot != null
        );

        public ICommand MoveBlipCommand => new RelayCommand
        (
            () => GrabBlip(ViewModel.ActiveBlip),
            () => m_wasContextMenuOpenedOnBlip
        );

        public ICommand CancelMoveBlipCommand => new RelayCommand
        (
            () => CancelMove(),
            () => m_grabbedBlip != null
        );

        public ICommand DeleteBlipCommand => new RelayCommand
        (
            () => ViewModel.DeleteBlip(ViewModel.ActiveBlip),
            () => (m_isContextMenuVisible && m_wasContextMenuOpenedOnBlip) ||
                  (!m_isContextMenuVisible && ViewModel.ActiveBlip != null)
        );

        public ICommand LocateBlipCommand => new RelayCommand
        (
            () => LocateBlip(ViewModel.ActiveBlip),
            () => ViewModel.ActiveBlip != null
        );

        private void ViewModel_BlipUpdate(object sender, RadarBlipEventArgs e)
        {
            switch (e.Action)
            {
                case BlipAction.Update:
                    foreach (var blip in e.Items) UpdateBlip(blip);
                    break;
                case BlipAction.Reset:
                    RemoveAllBlips();
                    break;
            }
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_grabbedBlip != null)
            {
                return;
            }

            MouseOverBlip = null;

            foreach (var item in ViewModel.RadarBlips)
            {
                if (!item.IsVisible)
                {
                    continue;
                }

                UIElement el = m_blipUIElementMap[item];
                Matrix m = el.RenderTransform.Value;
                Point p0 = m_map.PixelToMap(new Point(m.OffsetX, m.OffsetY));
                Point p1 = MouseOverMapCoords;
                double h = (el as FrameworkElement).Height;
                double w = (el as FrameworkElement).Width;

                if ((p0.X + w >= p1.X && p0.X <= p1.X) &&
                    (p0.Y + h >= p1.Y && p0.Y <= p1.Y))
                {
                    MouseOverBlip = item;
                    break;
                }
            }

            UpdateCursor();
        }

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseClickWorldCoords = MouseOverWorldCoords;

            if (m_grabbedBlip != null && e.ChangedButton == MouseButton.Right)
            {
                DropBlip(MouseClickWorldCoords);
                m_suppressContextMenuOnce = true;
            }

            if (MouseOverBlip != null)
            {
                ViewModel.ActiveBlip = MouseOverBlip;
            }
        }

        private void Location_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (m_mapTab.IsSelected && ViewModel.ActiveBlip != null)
            {
                if (e is PropertyChangedEventArgs<Vector3D> args)
                {
                    Vector3D oldV3 = ViewModel.ActiveBlip.MarkerPosition;
                    Vector3D newV3 = args.NewValue;
                    if (oldV3 != newV3) ViewModel.ActiveBlip.MarkerPosition = newV3;

                    Vector2D oldV2 = ViewModel.ActiveBlip.RadarPosition;
                    Vector2D newV2 = args.NewValue.Get2DComponent();
                    if (oldV2 != newV2) ViewModel.ActiveBlip.RadarPosition = newV2;
                }
            }
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_mapTab.IsSelected && ViewModel.ActiveBlip != null)
            {
                LocateBlip(ViewModel.ActiveBlip);
            }
        }

        private void ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            if (m_suppressContextMenuOnce)
            {
                e.Handled = true;
                m_suppressContextMenuOnce = false;
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            m_isContextMenuVisible = true;
            m_wasContextMenuOpenedOnBlip = (MouseOverBlip != null);
        }


        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            m_isContextMenuVisible = false;
        }

        private static readonly Dictionary<RadarBlipSprite, string> SpriteURIs = new Dictionary<RadarBlipSprite, string>()
        {
            { RadarBlipSprite.Asuka,    @"pack://application:,,,/Resources/radar_asuka.png"  },
            { RadarBlipSprite.Bomb,     @"pack://application:,,,/Resources/radar_bomb.png"   },
            { RadarBlipSprite.Cat,      @"pack://application:,,,/Resources/radar_cat.png"    },
            { RadarBlipSprite.Centre,   @"pack://application:,,,/Resources/radar_centre.png" },
            { RadarBlipSprite.Copcar,   @"pack://application:,,,/Resources/radar_copcar.png" },
            { RadarBlipSprite.Don,      @"pack://application:,,,/Resources/radar_don.png"    },
            { RadarBlipSprite.Eight,    @"pack://application:,,,/Resources/radar_eight.png"  },
            { RadarBlipSprite.El,       @"pack://application:,,,/Resources/radar_el.png"     },
            { RadarBlipSprite.Ice,      @"pack://application:,,,/Resources/radar_ice.png"    },
            { RadarBlipSprite.Joey,     @"pack://application:,,,/Resources/radar_joey.png"   },
            { RadarBlipSprite.Kenji,    @"pack://application:,,,/Resources/radar_kenji.png"  },
            { RadarBlipSprite.Liz,      @"pack://application:,,,/Resources/radar_liz.png"    },
            { RadarBlipSprite.Luigi,    @"pack://application:,,,/Resources/radar_luigi.png"  },
            { RadarBlipSprite.North,    @"pack://application:,,,/Resources/radar_north.png"  },
            { RadarBlipSprite.Ray,      @"pack://application:,,,/Resources/radar_ray.png"    },
            { RadarBlipSprite.Sal,      @"pack://application:,,,/Resources/radar_sal.png"    },
            { RadarBlipSprite.Save,     @"pack://application:,,,/Resources/radar_save.png"   },
            { RadarBlipSprite.Spray,    @"pack://application:,,,/Resources/radar_spray.png"  },
            { RadarBlipSprite.Tony,     @"pack://application:,,,/Resources/radar_tony.png"   },
            { RadarBlipSprite.Weapon,   @"pack://application:,,,/Resources/radar_weapon.png" },
        };

        public static ObservableCollection<ColorItem> StandardBlipColors => new ObservableCollection<ColorItem>()
        {
            new ColorItem(Color.FromRgb(0x7F, 0x00, 0x00), "Dark Red"),
            new ColorItem(Color.FromRgb(0x71, 0x2B, 0x49), "Red"),
            new ColorItem(Color.FromRgb(0x00, 0x7F, 0x00), "Dark Green"),
            new ColorItem(Color.FromRgb(0x5F, 0xA0, 0x6A), "Green"),
            new ColorItem(Color.FromRgb(0x00, 0x00, 0x7F), "Dark Blue"),
            new ColorItem(Color.FromRgb(0x80, 0xA7, 0xF3), "Blue"),
            new ColorItem(Color.FromRgb(0x7F, 0x7F, 0x7F), "Gray"),
            new ColorItem(Color.FromRgb(0xE1, 0xE1, 0xE1), "White"),
            new ColorItem(Color.FromRgb(0x7F, 0x7F, 0x00), "Dark Yellow"),
            new ColorItem(Color.FromRgb(0xFF, 0xFF, 0x00), "Yellow"),
            new ColorItem(Color.FromRgb(0x7F, 0x00, 0x7F), "Purple"),
            new ColorItem(Color.FromRgb(0xFF, 0x00, 0xFF), "Pink"),
            new ColorItem(Color.FromRgb(0x00, 0x7F, 0x7F), "Teal"),
            new ColorItem(Color.FromRgb(0x00, 0xFF, 0xFF), "Cyan"),
        };

        public static Color GetBlipColor(int colorId, bool isBright)
        {
            if (colorId >= 0 && colorId < NumStandardColorTypes)
            {
                int colorIndex = (isBright) ? (colorId * 2) + 1 : colorId * 2;
                return (Color) StandardBlipColors[colorIndex].Color;
            }

            // Interesting "feature" in the game
            byte r = (byte) (colorId >> 24);
            byte g = (byte) (colorId >> 16);
            byte b = (byte) (colorId >> 8);
            return Color.FromRgb(r, g, b);
        }
    }
}
