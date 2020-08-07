using GTA3SaveEditor.GUI.Helpers;
using GTA3SaveEditor.GUI.ViewModels;
using GTASaveData.GTA3;
using GTASaveData.Types;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfEssentials.Win32;
using Xceed.Wpf.Toolkit.Core;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : TabPageBase<Player>
    {
        public const double PointOfInterestRadius = 10;

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register(
            nameof(Skin), typeof(BitmapImage), typeof(PlayerView));

        public static readonly DependencyProperty SelectedPointOfInterestProperty = DependencyProperty.Register(
            nameof(SelectedPointOfInterest), typeof(string), typeof(PlayerView));

        public static readonly DependencyProperty MouseClickCoordsProperty = DependencyProperty.Register(
            nameof(MouseClickCoords), typeof(Point), typeof(PlayerView));

        public static readonly DependencyProperty MouseOverCoordsProperty = DependencyProperty.Register(
            nameof(MouseOverCoords), typeof(Point), typeof(PlayerView));

        public static readonly DependencyProperty BlipsProperty = DependencyProperty.Register(
            nameof(Blips), typeof(ObservableCollection<UIElement>), typeof(PlayerView),
            new PropertyMetadata(
                new ObservableCollection<UIElement>()));

        public BitmapImage Skin
        {
            get { return (BitmapImage) GetValue(SkinProperty); }
            set { SetValue(SkinProperty, value); }
        }

        public string SelectedPointOfInterest
        {
            get { return (string) GetValue(SelectedPointOfInterestProperty); }
            set { SetValue(SelectedPointOfInterestProperty, value); }
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

        private bool m_suppressPointOfInterestSelectionChangedHandler;

        public PlayerView()
        {
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            m_map.Reset();
        }

        private void LoadSkin(string model)
        {
            if (SkinUriMap.TryGetValue(model, out string uri))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.DecodePixelWidth = 420;
                img.UriSource = new Uri(uri);
                img.EndInit();
                Skin = img;
            }
            else
            {
                Skin = null;
            }
        }

        private void UpdateBlip()
        {
            UIElement blip = MapHelper.MakeBlip(
                ViewModel.PlayerPed.Position.Get2DComponent(),
                scale: 6, thickness: 1.5,
                color: 6
            );

            if (Blips.Count == 0) Blips.Add(blip);
            else Blips[0] = blip;
        }

        private void UpdatePointOfInterest()
        {
            m_suppressPointOfInterestSelectionChangedHandler = true;
            SelectedPointOfInterest = PointsOfInterest
                .Where(x => Vector3D.Distance(x.Value, ViewModel.PlayerPed.Position) < PointOfInterestRadius)
                .Select(x => x.Key)
                .FirstOrDefault();
            m_suppressPointOfInterestSelectionChangedHandler = false;
        }

        private void SetSpawnPoint2D(Point p)
        {
            Vector3D oldPos = ViewModel.PlayerPed.Position;
            Vector3D newPos = new Vector3D((float) p.X, (float) p.Y, (float) oldPos.Z);

            TheEditor.SetPlayerSpawnPoint(newPos);
        }

        private void PlayerPostionPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (e is PropertyChangedEventArgs<Vector3D> args)
            {
                Vector3D oldPos = ViewModel.PlayerPed.Position;
                Vector3D newPos = args.NewValue;
                if (oldPos != newPos)
                {
                    TheEditor.SetPlayerSpawnPoint(newPos);
                }
            }

            UpdateBlip();
            UpdatePointOfInterest();
        }

        private void PointOfInterest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0 && !m_suppressPointOfInterestSelectionChangedHandler)
            {
                Vector3D loc = PointsOfInterest[e.AddedItems[0] as string];
                if (Vector3D.Distance(loc, ViewModel.PlayerPed.Position) > PointOfInterestRadius)
                {
                    TheEditor.SetPlayerSpawnPoint(loc);
                }
            }
        }

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseClickCoords = MouseOverCoords;
        }

        private void SkinComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadSkin(ViewModel.PlayerPed.ModelName);
        }

        public ICommand SpawnHereCommand => new RelayCommand
        (
            () => SetSpawnPoint2D(MouseClickCoords)
        );

        private static readonly Dictionary<string, string> SkinUriMap = new Dictionary<string, string>()
        {
            { "asuka",      @"pack://application:,,,/Resources/Skins/asuka.png" },
            { "bomber",     @"pack://application:,,,/Resources/Skins/bomber.png" },
            { "butler",     @"pack://application:,,,/Resources/Skins/butler.png" },
            { "cat",        @"pack://application:,,,/Resources/Skins/cat.png" },
            { "chunky",     @"pack://application:,,,/Resources/Skins/chunky.png" },
            { "col1",       @"pack://application:,,,/Resources/Skins/col1.png" },
            { "col2",       @"pack://application:,,,/Resources/Skins/col2.png" },
            { "col3",       @"pack://application:,,,/Resources/Skins/col3.png" },
            { "colrob",     @"pack://application:,,,/Resources/Skins/colrob.png" },
            { "cop2",       @"pack://application:,,,/Resources/Skins/cop2.png" },
            { "curly",      @"pack://application:,,,/Resources/Skins/curly.png" },
            { "darkel",     @"pack://application:,,,/Resources/Skins/darkel.png" },
            { "dealer",     @"pack://application:,,,/Resources/Skins/dealer.png" },
            { "donky",      @"pack://application:,,,/Resources/Skins/donky.png" },
            { "eight",      @"pack://application:,,,/Resources/Skins/eight.png" },
            { "eight2",     @"pack://application:,,,/Resources/Skins/eight2.png" },
            { "frankie",    @"pack://application:,,,/Resources/Skins/frankie.png" },
            { "goon",       @"pack://application:,,,/Resources/Skins/goon.png" },
            { "joey",       @"pack://application:,,,/Resources/Skins/joey.png" },
            { "joey2",      @"pack://application:,,,/Resources/Skins/joey2.png" },
            { "keeper",     @"pack://application:,,,/Resources/Skins/keeper.png" },
            { "kenji",      @"pack://application:,,,/Resources/Skins/kenji.png" },
            { "lips",       @"pack://application:,,,/Resources/Skins/lips.png" },
            { "love",       @"pack://application:,,,/Resources/Skins/love.png" },
            { "love2",      @"pack://application:,,,/Resources/Skins/love2.png" },
            { "luigi",      @"pack://application:,,,/Resources/Skins/luigi.png" },
            { "maria",      @"pack://application:,,,/Resources/Skins/maria.png" },
            { "mickey",     @"pack://application:,,,/Resources/Skins/mickey.png" },
            { "miguel",     @"pack://application:,,,/Resources/Skins/miguel.png" },
            { "misty",      @"pack://application:,,,/Resources/Skins/misty.png" },
            { "ojg",        @"pack://application:,,,/Resources/Skins/ojg.png" },
            { "ojg2",       @"pack://application:,,,/Resources/Skins/ojg2.png" },
            { "ojg_p",      @"pack://application:,,,/Resources/Skins/ojg_p.png" },
            { "player",     @"pack://application:,,,/Resources/Skins/player.png" },
            { "playerp",    @"pack://application:,,,/Resources/Skins/playerp.png" },
            { "playerx",    @"pack://application:,,,/Resources/Skins/playerx.png" },
            { "ray",        @"pack://application:,,,/Resources/Skins/ray.png" },
            { "robber",     @"pack://application:,,,/Resources/Skins/robber.png" },
            { "s_guard",    @"pack://application:,,,/Resources/Skins/s_guard.png" },
            { "sam",        @"pack://application:,,,/Resources/Skins/sam.png" },
            { "tanner",     @"pack://application:,,,/Resources/Skins/tanner.png" },
            { "tony",       @"pack://application:,,,/Resources/Skins/tony.png" },
        };

        public static IEnumerable<string> ModelNames => SkinUriMap.Keys;

        public static readonly Dictionary<string, Vector3D> PointsOfInterest = new Dictionary<string, Vector3D>()
        {
            { "Portland Safehouse", new Vector3D(888.5625f, -308.375f, 8.0f) },
            { "Staunton Safehouse", new Vector3D(103.0f, -478.5f, 14.875f) },
            { "Shoreside Safehouse", new Vector3D(-666.75f, -1.75f, 18.0f) },
            { "Construction Site Tower", new Vector3D(378.75f, -320.75f, 66.0f) },
            { "Love Media Rooftop", new Vector3D(100.0f, -1555.0f, 245.0f) },
            { "Kenji's Rooftop", new Vector3D(474.5f, -1383.5f, 69.0f) },
            { "Lift Bridge", new Vector3D(-266.5f, -631.5f, 72.5f) }
        };

        public static IEnumerable<string> PointOfInterestNames => PointsOfInterest.Keys;
    }
}
