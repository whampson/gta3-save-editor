using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using GTASaveData.Types;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using WpfEssentials;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Radar : TabPageViewModelBase
    {
        public event EventHandler<BlipEventArgs<RadarBlip>> BlipUpdate;

        private readonly Random m_rand;
        private Array<RadarBlip> m_radarBlips;
        private RadarBlip m_activeBlip;

        public Array<RadarBlip> RadarBlips
        {
            get { return m_radarBlips; }
            set { m_radarBlips = value; OnPropertyChanged(); }
        }

        public RadarBlip ActiveBlip
        {
            get { return m_activeBlip; }
            set { m_activeBlip = value; OnPropertyChanged(); }
        }

        public RadarBlip NextAvailableSlot
        {
            get
            {
                if (RadarBlips == null) return null;
                return RadarBlips.FirstOrDefault(x => !x.IsVisible);
            }
        }

        public Radar(Main mainViewModel)
            : base("Radar", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        {
            m_rand = new Random((int) DateTime.Now.Ticks);
        }

        public override void Load()
        {
            base.Load();

            RadarBlips = MainViewModel.TheSave.RadarBlips.RadarBlips;
            RadarBlips.CollectionChanged += RadarBlips_CollectionChanged;
            RadarBlips.ItemStateChanged += RadarBlips_ItemStateChanged;
            DrawAllBlips();
        }

        public override void Unload()
        {
            base.Unload();

            RemoveAllBlips();
            RadarBlips.ItemStateChanged -= RadarBlips_ItemStateChanged;
            RadarBlips.CollectionChanged -= RadarBlips_CollectionChanged;
            RadarBlips = null;
        }

        public void CreateBlip(Point loc)
        {
            RadarBlip blip = NextAvailableSlot;
            blip.Display = RadarBlipDisplay.Blip;
            blip.Type = RadarBlipType.Coord;
            blip.Sprite = RadarBlipSprite.None;
            blip.Scale = 3;
            blip.Color = 1;
            blip.IsBright = true;
            blip.IsInUse = true;
            blip.Index = GenerateNewIndex();
            MoveBlip(blip, loc);

            ActiveBlip = blip;
        }

        public void MoveBlip(RadarBlip blip, Point loc)
        {
            Vector3D old = blip.MarkerPosition;

            Vector2D r = new Vector2D() { X = (float) loc.X, Y = (float) loc.Y };
            Vector3D m = new Vector3D() { X = (float) loc.X, Y = (float) loc.Y, Z = old.Z };

            blip.RadarPosition = r;
            blip.MarkerPosition = m;
        }

        public void DeleteBlip(RadarBlip blip)
        {
            blip.Invalidate();
        }

        private void UpdateBlip(RadarBlip newBlip)
        {
            BlipUpdate?.Invoke(this, new BlipEventArgs<RadarBlip>()
            {
                Action = BlipAction.Update,
                Items = { newBlip }
            });
        }

        public void DrawAllBlips()
        {
            BlipUpdate?.Invoke(this, new BlipEventArgs<RadarBlip>()
            {
                Action = BlipAction.Update,
                Items = RadarBlips
            });
        }

        public void RemoveAllBlips()
        {
            BlipUpdate?.Invoke(this, new BlipEventArgs<RadarBlip>()
            {
                Action = BlipAction.Reset
            });
        }

        private short GenerateNewIndex()
        {
            short index;
            do
            {
                index = (short) m_rand.Next(1, short.MaxValue);
            } while (RadarBlips.Any(x => x.Index == index));

            return index;
        }

        private void RadarBlips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                {
                    BlipUpdate?.Invoke(this, new BlipEventArgs<RadarBlip>()
                    {
                        Action = BlipAction.Update,
                        Items = e.NewItems.Cast<RadarBlip>().ToList()
                    });
                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    RemoveAllBlips();
                    break;
                }
            }
        }

        private void RadarBlips_ItemStateChanged(object sender, ItemStateChangedEventArgs e)
        {
            UpdateBlip(RadarBlips[e.ItemIndex]);
        }
    }
}
