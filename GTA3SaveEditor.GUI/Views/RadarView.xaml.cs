using GTA3SaveEditor.GUI.Controls;
using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.ViewModels;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfEssentials;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for RadarView.xaml
    /// </summary>
    public partial class RadarView : TabPageBase<Radar>
    {
        public static readonly DependencyProperty BlipsProperty = DependencyProperty.Register(
            nameof(Blips), typeof(ObservableCollection<UIElement>), typeof(MapControl),
            new PropertyMetadata(
                new ObservableCollection<UIElement>()));

        public ObservableCollection<UIElement> Blips
        {
            get { return (ObservableCollection<UIElement>) GetValue(BlipsProperty); }
            set { SetValue(BlipsProperty, value); }
        }

        private Dictionary<int, UIElement> m_blipUIElementMap;

        public RadarView()
        {
            m_blipUIElementMap = new Dictionary<int, UIElement>();
            InitializeComponent();
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

        private void ViewModel_BlipUpdate(object sender, RadarBlipEventArgs e)
        {
            switch (e.Action)
            {
                case BlipAction.Add:
                    foreach (RadarBlip blip in e.NewItems) AddBlip(blip);
                    break;
                case BlipAction.Remove:
                    foreach (RadarBlip blip in e.OldItems) RemoveBlip(blip);
                    break;
                case BlipAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++) ReplaceBlip(e.OldStartingIndex + i, e.NewItems[i]);
                    break;
                case BlipAction.Reset:
                    RemoveAllBlips();
                    break;
            }
        }

        private void AddBlip(RadarBlip blip)
        {
            if (blip.InUse)
            {
                UIElement e = (blip.Sprite == RadarBlipSprite.None)
                    ? MakeBlip(blip)
                    : MakeSpriteBlip(blip, SpriteURIs[blip.Sprite]);
                
                Blips.Add(e);
                m_blipUIElementMap[blip.BlipIndex] = e;
            }
        }

        private UIElement MakeBlip(RadarBlip blip)
        {
            const int Size = 2;

            Rectangle rect = new Rectangle();
            SolidColorBrush brush = new SolidColorBrush();

            int colorId = blip.ColorId;
            if (colorId >= 0 && colorId < BlipColors.Count)
            {
                var c = BlipColors[blip.ColorId];
                brush.Color = (blip.Dim) ? c.Item1 : c.Item2;       // TODO: BUG: blip.Dim is inverted?
            }
            else
            {
                // TOOD: test this in-game
                byte a = (byte) (blip.ColorId >> 24);
                byte r = (byte) (blip.ColorId >> 16);
                byte g = (byte) (blip.ColorId >> 8);
                byte b = (byte) blip.ColorId;
                brush.Color = Color.FromArgb(a, r, g, b);
            }

            rect.Fill = brush;
            rect.StrokeThickness = 0.5;
            rect.Stroke = Brushes.Black;

            // TODO: test scale
            rect.Width = Size * blip.Scale;
            rect.Height = Size * blip.Scale;

            Point p = m_map.WorldToPixel(new Point(blip.RadarPosition.X, blip.RadarPosition.Y));

            Matrix m = Matrix.Identity;
            m.OffsetX = p.X - (rect.Width / 2);
            m.OffsetY = p.Y - (rect.Width / 2);

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
            m.ScalePrepend(blip.Scale, blip.Scale);

            img.RenderTransform = new MatrixTransform(m);
            return img;
        }

        private void ReplaceBlip(int index, RadarBlip newBlip)
        {
            Blips.Remove(m_blipUIElementMap[index]);
            AddBlip(newBlip);
        }

        private void RemoveBlip(RadarBlip blip)
        {
            if (m_blipUIElementMap.TryGetValue(blip.BlipIndex, out UIElement e))
            {
                Blips.Remove(e);
                m_blipUIElementMap.Remove(blip.BlipIndex);
            }
        }

        private void RemoveAllBlips()
        {
            Blips.Clear();
            m_blipUIElementMap.Clear();
        }

        private static readonly Dictionary<RadarBlipSprite, string> SpriteURIs = new Dictionary<RadarBlipSprite, string>()
        {
            { RadarBlipSprite.Asuka,    @"pack://application:,,,/Resources/radar_asuka.png"  },
            { RadarBlipSprite.Bomb,     @"pack://application:,,,/Resources/radar_bomb.png"   },
            { RadarBlipSprite.Cat,      @"pack://application:,,,/Resources/radar_cat.png"    },
            { RadarBlipSprite.Center,   @"pack://application:,,,/Resources/radar_centre.png" },
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

        private static readonly List<(Color, Color)> BlipColors = new List<(Color, Color)>()
        {
            // Bright                         Dim
            (Color.FromRgb(0x71, 0x2B, 0x49), Color.FromRgb(0x7F, 0x00, 0x00)), // Red
            (Color.FromRgb(0x5F, 0xA0, 0x6A), Color.FromRgb(0x00, 0x7F, 0x00)), // Green
            (Color.FromRgb(0x80, 0xA7, 0xF3), Color.FromRgb(0x00, 0x00, 0x7F)), // Blue
            (Color.FromRgb(0xE1, 0xE1, 0xE1), Color.FromRgb(0x7F, 0x7F, 0x7F)), // White
            (Color.FromRgb(0xFF, 0xFF, 0x00), Color.FromRgb(0x7F, 0x7F, 0x00)), // Yellow
            (Color.FromRgb(0xFF, 0x00, 0xFF), Color.FromRgb(0x7F, 0x00, 0x7F)), // Purple
            (Color.FromRgb(0x00, 0xFF, 0xFF), Color.FromRgb(0x00, 0x7F, 0x7F)), // Cyan
        };
    }
}
