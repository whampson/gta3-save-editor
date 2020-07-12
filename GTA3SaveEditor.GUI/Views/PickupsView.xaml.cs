using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.Helpers;
using GTA3SaveEditor.GUI.ViewModels;
using GTASaveData.GTA3;
using GTASaveData.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for PickupsView.xaml
    /// </summary>
    public partial class PickupsView : TabPageBase<Pickups>
    {
        public static readonly DependencyProperty InfoVisibleProperty = DependencyProperty.Register(
            nameof(InfoVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty HealthVisibleProperty = DependencyProperty.Register(
            nameof(HealthVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty ArmorVisibleProperty = DependencyProperty.Register(
            nameof(ArmorVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty AdrenalineVisibleProperty = DependencyProperty.Register(
            nameof(AdrenalineVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty BribesVisibleProperty = DependencyProperty.Register(
            nameof(BribesVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty RampagesVisibleProperty = DependencyProperty.Register(
            nameof(RampagesVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty WeaponsVisibleProperty = DependencyProperty.Register(
            nameof(WeaponsVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty PackagesVisibleProperty = DependencyProperty.Register(
            nameof(PackagesVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty MiscVisibleProperty = DependencyProperty.Register(
            nameof(MiscVisible), typeof(bool), typeof(PickupsView), new PropertyMetadata(true));

        public static readonly DependencyProperty BlipScaleProperty = DependencyProperty.Register(
            nameof(BlipScale), typeof(int), typeof(PickupsView));

        public static readonly DependencyProperty BlipAngleProperty = DependencyProperty.Register(
            nameof(BlipAngle), typeof(int), typeof(PickupsView));

        public static readonly DependencyProperty MouseClickCoordsProperty = DependencyProperty.Register(
            nameof(MouseClickCoords), typeof(Point), typeof(PickupsView));

        public static readonly DependencyProperty MouseOverCoordsProperty = DependencyProperty.Register(
            nameof(MouseOverCoords), typeof(Point), typeof(PickupsView));

        public static readonly DependencyProperty BlipsProperty = DependencyProperty.Register(
            nameof(Blips), typeof(ObservableCollection<UIElement>), typeof(PickupsView),
            new PropertyMetadata(
                new ObservableCollection<UIElement>()));

        public bool InfoVisible
        {
            get { return (bool) GetValue(InfoVisibleProperty); }
            set { SetValue(InfoVisibleProperty, value); }
        }

        public bool HealthVisible
        {
            get { return (bool) GetValue(HealthVisibleProperty); }
            set { SetValue(HealthVisibleProperty, value); }
        }

        public bool ArmorVisible
        {
            get { return (bool) GetValue(ArmorVisibleProperty); }
            set { SetValue(ArmorVisibleProperty, value); }
        }

        public bool AdrenalineVisible
        {
            get { return (bool) GetValue(AdrenalineVisibleProperty); }
            set { SetValue(AdrenalineVisibleProperty, value); }
        }

        public bool BribesVisible
        {
            get { return (bool) GetValue(BribesVisibleProperty); }
            set { SetValue(BribesVisibleProperty, value); }
        }

        public bool RampagesVisible
        {
            get { return (bool) GetValue(RampagesVisibleProperty); }
            set { SetValue(RampagesVisibleProperty, value); }
        }

        public bool WeaponsVisible
        {
            get { return (bool) GetValue(WeaponsVisibleProperty); }
            set { SetValue(WeaponsVisibleProperty, value); }
        }

        public bool PackagesVisible
        {
            get { return (bool) GetValue(PackagesVisibleProperty); }
            set { SetValue(PackagesVisibleProperty, value); }
        }

        public bool MiscVisible
        {
            get { return (bool) GetValue(MiscVisibleProperty); }
            set { SetValue(MiscVisibleProperty, value); }
        }

        public int BlipScale
        {
            get { return (int) GetValue(BlipScaleProperty); }
            set { SetValue(BlipScaleProperty, value); }
        }

        public int BlipAngle
        {
            get { return (int) GetValue(BlipAngleProperty); }
            set { SetValue(BlipAngleProperty, value); }
        }

        public Point MouseClickCoords
        {
            get { return (Point) GetValue(MouseClickCoordsProperty); }
            set { SetValue(MouseClickCoordsProperty, value); }
        }

        public Point MouseOverCoords
        {
            get { return (Point) GetValue(MouseOverCoordsProperty); }
            set { SetValue(MouseOverCoordsProperty, value); }
        }

        public ObservableCollection<UIElement> Blips
        {
            get { return (ObservableCollection<UIElement>) GetValue(BlipsProperty); }
            set { SetValue(BlipsProperty, value); }
        }

        private readonly Dictionary<Pickup, UIElement> m_blipUIElementMap;

        public PickupsView()
        {
            m_blipUIElementMap = new Dictionary<Pickup, UIElement>();
            InitializeComponent();

            BlipScale = 4;
            BlipAngle = 0;
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

        private void UpdateAllBlips()
        {
            foreach (var blip in ViewModel.PickupsArray) UpdateBlip(blip);
        }

        private void UpdateBlip(Pickup blip)
        {
            if (m_blipUIElementMap.TryGetValue(blip, out UIElement oldElement))
            {
                Blips.Remove(oldElement);
                m_blipUIElementMap.Remove(blip);
            }

            if (blip.Type == PickupType.None || blip.ObjectIndex == 0 ||
                (IsInfo(blip) && !InfoVisible) ||
                (IsHealth(blip) && !HealthVisible) ||
                (IsArmor(blip) && !ArmorVisible) ||
                (IsAdrenaline(blip) && !AdrenalineVisible) ||
                (IsBribe(blip) && !BribesVisible) ||
                (IsRampage(blip) && !RampagesVisible) ||
                (IsWeapon(blip) && !WeaponsVisible) ||
                (IsPackage(blip) && !PackagesVisible) ||
                (IsMisc(blip) && !MiscVisible))
            {
                return;
            }

            Vector2D loc = blip.Position.Get2DComponent();
            BlipToolTips.TryGetValue(blip.ModelIndex, out string toolTip);

            UIElement newElement = (SpriteURIs.TryGetValue(blip.ModelIndex, out string uri))
                ? MapHelper.MakeIconBlip(
                    loc, uri,
                    scale: BlipScale, angle: BlipAngle,
                    toolTip: toolTip)
                : MapHelper.MakeBlip(
                    loc,
                    scale: BlipScale, thickness: 0.75, angle: BlipAngle,
                    color: blip.ModelIndex % 6, isBright: (blip.ModelIndex % 12) >= 6,
                    toolTip: toolTip);

            Blips.Add(newElement);
            m_blipUIElementMap[blip] = newElement;
        }

        private void ViewModel_BlipUpdate(object sender, BlipEventArgs<Pickup> e)
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

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseClickCoords = MouseOverCoords;
        }

        private void FilterCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                UpdateAllBlips();
            }
        }

        private void BlipSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ViewModel != null)
            {
                UpdateAllBlips();
            }
        }

        // TODO: read gta3.ide and default.ide for model indices

        private static bool IsInfo(Pickup p) => p.ModelIndex == 1361;
        private static bool IsHealth(Pickup p) => p.ModelIndex == 1362;
        private static bool IsArmor(Pickup p) => p.ModelIndex == 1364;
        private static bool IsAdrenaline(Pickup p) => p.ModelIndex == 1363;
        private static bool IsBribe(Pickup p) => p.ModelIndex == 1383;
        private static bool IsRampage(Pickup p) => p.ModelIndex == 1392;
        private static bool IsPackage(Pickup p) => p.ModelIndex == 1321;
        private static bool IsWeapon(Pickup p) =>
            p.ModelIndex == 170 ||  // Grenade
            p.ModelIndex == 171 ||  // AK47
            p.ModelIndex == 172 ||  // Baseball Bat
            p.ModelIndex == 173 ||  // Pistol
            p.ModelIndex == 174 ||  // Molotov
            p.ModelIndex == 175 ||  // Rocket Launcher
            p.ModelIndex == 176 ||  // Shotgun
            p.ModelIndex == 177 ||  // Sniper Rifle
            p.ModelIndex == 178 ||  // Uzi
            p.ModelIndex == 180 ||  // M16
            p.ModelIndex == 181;    // Flamethrower
        // TODO: detonator?

        private static bool IsMisc(Pickup p) =>
            !IsInfo(p) &&
            !IsHealth(p) &&
            !IsArmor(p) &&
            !IsAdrenaline(p) &&
            !IsBribe(p) &&
            !IsRampage(p) &&
            !IsPackage(p) &&
            !IsWeapon(p);

        public static readonly Dictionary<int, string> SpriteURIs = new Dictionary<int, string>()
        {
            { 170, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 171, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 172, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 173, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 174, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 175, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 176, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 177, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 178, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 180, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 181, @"pack://application:,,,/Resources/Map/radar_weapon.png" },
            { 1321, @"pack://application:,,,/Resources/Map/package.png" },
            { 1361, @"pack://application:,,,/Resources/Map/info.png" },
            { 1362, @"pack://application:,,,/Resources/Map/health.png" },
            { 1363, @"pack://application:,,,/Resources/Map/adrenaline.png" },
            { 1364, @"pack://application:,,,/Resources/Map/bodyarmour.png" },
            { 1383, @"pack://application:,,,/Resources/Map/bribe.png" },
            { 1384, @"pack://application:,,,/Resources/Map/bonus.png" },
            { 1392, @"pack://application:,,,/Resources/Map/killfrenzy.png" },
        };

        public static readonly Dictionary<int, string> BlipToolTips = new Dictionary<int, string>()
        {
            { 170, "Grenade" },
            { 171, "AK47" },
            { 172, "Baseball Bat" },
            { 173, "Pistol" },
            { 174, "Molotov Cocktail" },
            { 175, "Rocket Launcher" },
            { 176, "Shotgun" },
            { 177, "Sniper Rifle" },
            { 178, "Uzi" },
            { 180, "M16" },
            { 181, "Flamethrower" },
            { 1321, "Hidden Package" },
            { 1361, "Info" },
            { 1362, "Health" },
            { 1363, "Adrenaline" },
            { 1364, "Armor" },
            { 1383, "Police Bribe" },
            { 1384, "Bonus" },
            { 1392, "Rampage" },
        };
    }
}
