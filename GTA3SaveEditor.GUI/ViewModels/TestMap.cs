using GTASaveData.GTA3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class TestMap : TabPageViewModelBase
    {
        private double m_zoomLevel;
        private Point m_panOffset;
        private Point m_contextMenuOffset;
        private Point m_contextMenuCoords;
        private Point m_mouseOverCoords;
        private ObservableCollection<MapBlip> m_blips;
        private bool m_isMouseOverBlip;
        private bool m_isBlipSelected;
        private MapBlip m_selectedBlip;

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
            get { return m_contextMenuOffset; }
            set { m_contextMenuOffset = value; OnPropertyChanged(); }
        }

        public Point ContextMenuCoords
        {
            get { return m_contextMenuCoords; }
            set { m_contextMenuCoords = value; OnPropertyChanged(); }
        }

        public Point MouseOverCoords
        {
            get { return m_mouseOverCoords; }
            set { m_mouseOverCoords = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MapBlip> Blips
        {
            get { return m_blips; }
            set { m_blips = value; OnPropertyChanged(); }
        }

        public bool IsMouseOverBlip
        {
            get { return m_isMouseOverBlip; }
            set { m_isMouseOverBlip = value; OnPropertyChanged(); }
        }

        public bool IsBlipSelected
        {
            get { return m_isBlipSelected; }
            set { m_isBlipSelected = value; OnPropertyChanged(); }
        }

        public MapBlip SelectedBlip
        {
            get { return m_selectedBlip; }
            set { m_selectedBlip = value; OnPropertyChanged(); }
        }

        public TestMap(Main mainViewModel)
            : base("Test Map", TabPageVisibility.Always, mainViewModel)
        {
            Blips = new ObservableCollection<MapBlip>();
        }

        public void MoveSelectedBlip()
        {
            if (!IsBlipSelected)
            {
                return;
            }


        }

        public ICommand AddAsukaBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                BlipSprites[RadarBlipSprite.Asuka],
                toolTip: "Asuka"))
        );
        public ICommand AddCenterBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                BlipSprites[RadarBlipSprite.Center],
                toolTip: "Player"))
        );
        public ICommand AddSaveBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                BlipSprites[RadarBlipSprite.Save],
                toolTip: "Safehouse"))
        );
        public ICommand AddCopCarBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                BlipSprites[RadarBlipSprite.Copcar],
                toolTip: "Police Station"))
        );
        public ICommand AddBribeBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                @"pack://application:,,,/Resources/bribe.png",
                decodeSize: 64,
                toolTip: "Police Bribe"))
        );
        public ICommand AddHeartBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                @"pack://application:,,,/Resources/health.png",
                decodeSize: 64,
                toolTip: "Health"))
        );
        public ICommand AddMedicBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                @"pack://application:,,,/Resources/medic.png",
                decodeSize: 32,
                toolTip: "Hospital"))
        );
        public ICommand AddSkullBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                @"pack://application:,,,/Resources/rampage.png",
                decodeSize: 64,
                toolTip: "Rampage"))
        );
        public ICommand AddPackageBlipHereCommand => new RelayCommand
        (
            () => Blips.Add(MapBlip.CreateFromSprite(
                ContextMenuCoords,
                @"pack://application:,,,/Resources/package.png",
                decodeSize: 64,
                toolTip: "Hidden Package"))
        );
        public ICommand MoveBlipCommand => new RelayCommand
        (
            () => MoveSelectedBlip(),
            () => IsBlipSelected
        );
        public ICommand RemoveBlipCommand => new RelayCommand
        (
            () => Blips.Remove(SelectedBlip),
            () => IsBlipSelected
        );

        public static readonly Dictionary<RadarBlipSprite, string> BlipSprites = new Dictionary<RadarBlipSprite, string>()
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
    }
}
