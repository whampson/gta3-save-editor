﻿using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.Helpers;
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
                    ? MapHelper.MakeBlip(blip.RadarPosition, scale: blip.Scale, color: blip.Color, isBright: blip.IsBright)
                    : MapHelper.MakeIconBlip(blip.RadarPosition, SpriteURIs[blip.Sprite]);

                Blips.Add(newElement);
                m_blipUIElementMap[blip] = newElement;
            }
        }

        private void LocateBlip(RadarBlip blip)
        {
            MapPan = m_map.WorldToMap(new Point(-blip.RadarPosition.X, -blip.RadarPosition.Y));
            MapZoom = 3;
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

        private void ViewModel_BlipUpdate(object sender, BlipEventArgs<RadarBlip> e)
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
                if (ViewModel.ActiveBlip.IsVisible)
                {
                    LocateBlip(ViewModel.ActiveBlip);
                }
                else
                {
                    m_map.Reset();
                }
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
            { RadarBlipSprite.Asuka,    @"pack://application:,,,/Resources/Map/radar_asuka.png"  },
            { RadarBlipSprite.Bomb,     @"pack://application:,,,/Resources/Map/radar_bomb.png"   },
            { RadarBlipSprite.Cat,      @"pack://application:,,,/Resources/Map/radar_cat.png"    },
            { RadarBlipSprite.Centre,   @"pack://application:,,,/Resources/Map/radar_centre.png" },
            { RadarBlipSprite.Copcar,   @"pack://application:,,,/Resources/Map/radar_copcar.png" },
            { RadarBlipSprite.Don,      @"pack://application:,,,/Resources/Map/radar_don.png"    },
            { RadarBlipSprite.Eight,    @"pack://application:,,,/Resources/Map/radar_eight.png"  },
            { RadarBlipSprite.El,       @"pack://application:,,,/Resources/Map/radar_el.png"     },
            { RadarBlipSprite.Ice,      @"pack://application:,,,/Resources/Map/radar_ice.png"    },
            { RadarBlipSprite.Joey,     @"pack://application:,,,/Resources/Map/radar_joey.png"   },
            { RadarBlipSprite.Kenji,    @"pack://application:,,,/Resources/Map/radar_kenji.png"  },
            { RadarBlipSprite.Liz,      @"pack://application:,,,/Resources/Map/radar_liz.png"    },
            { RadarBlipSprite.Luigi,    @"pack://application:,,,/Resources/Map/radar_luigi.png"  },
            { RadarBlipSprite.North,    @"pack://application:,,,/Resources/Map/radar_north.png"  },
            { RadarBlipSprite.Ray,      @"pack://application:,,,/Resources/Map/radar_ray.png"    },
            { RadarBlipSprite.Sal,      @"pack://application:,,,/Resources/Map/radar_sal.png"    },
            { RadarBlipSprite.Save,     @"pack://application:,,,/Resources/Map/radar_save.png"   },
            { RadarBlipSprite.Spray,    @"pack://application:,,,/Resources/Map/radar_spray.png"  },
            { RadarBlipSprite.Tony,     @"pack://application:,,,/Resources/Map/radar_tony.png"   },
            { RadarBlipSprite.Weapon,   @"pack://application:,,,/Resources/Map/radar_weapon.png" },
        };
    }
}
