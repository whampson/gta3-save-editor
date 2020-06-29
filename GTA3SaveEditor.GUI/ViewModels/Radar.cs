using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using WpfEssentials;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Radar : TabPageViewModelBase
    {
        public event EventHandler<RadarBlipEventArgs> BlipUpdate;

        private Array<RadarBlip> m_radarBlips;
        private Point m_mouseOverMapCoords;
        private Point m_mouseOverWorldCoords;

        public Array<RadarBlip> RadarBlips
        {
            get { return m_radarBlips; }
            set { m_radarBlips = value; OnPropertyChanged(); }
        }

        public Point MouseOverMapCoords
        {
            get { return m_mouseOverMapCoords; }
            set { m_mouseOverMapCoords = value; OnPropertyChanged(); }
        }

        public Point MouseOverWorldCoords
        {
            get { return m_mouseOverWorldCoords; }
            set { m_mouseOverWorldCoords = value; OnPropertyChanged(); }
        }

        public Radar(Main mainViewModel)
            : base("Radar", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

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

            EraseAllBlips();
            RadarBlips.ItemStateChanged -= RadarBlips_ItemStateChanged;
            RadarBlips.CollectionChanged -= RadarBlips_CollectionChanged;
            RadarBlips = null;
        }

        public void DrawAllBlips()
        {
            BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
            {
                Action = BlipAction.Add,
                NewItems = RadarBlips
            });
        }

        public void EraseAllBlips()
        {
            BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
            {
                Action = BlipAction.Reset
            });
        }

        //private void DrawBlip(RadarBlip blip)
        //{
        //    BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
        //    {
        //        Action = BlipAction.Add,
        //        NewItems = { blip }
        //    });
        //}

        //private void EraseBlip(RadarBlip blip)
        //{
        //    BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
        //    {
        //        Action = BlipAction.Remove,
        //        OldItems = { blip }
        //    });
        //}

        private void ReplaceBlip(int index, RadarBlip newBlip)
        {
            BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
            {
                Action = BlipAction.Replace,
                OldStartingIndex = index,
                NewItems = { newBlip }
            });
        }

        private void RadarBlips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
                    {
                        Action = BlipAction.Remove,
                        NewItems = e.NewItems.Cast<RadarBlip>().ToList(),
                    });
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
                    {
                        Action = BlipAction.Remove,
                        OldItems = e.OldItems.Cast<RadarBlip>().ToList()
                    });
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                {
                    BlipUpdate?.Invoke(this, new RadarBlipEventArgs()
                    {
                        Action = BlipAction.Replace,
                        OldItems = e.NewItems.Cast<RadarBlip>().ToList()
                    });
                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    EraseAllBlips();
                    break;
                }
            }
        }

        private void RadarBlips_ItemStateChanged(object sender, ItemStateChangedEventArgs e)
        {
            ReplaceBlip(e.ItemIndex, RadarBlips[e.ItemIndex]);
        }
    }
}
