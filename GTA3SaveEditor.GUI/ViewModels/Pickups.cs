using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.Specialized;
using System.Linq;
using WpfEssentials;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Pickups : TabPageViewModelBase
    {
        public event EventHandler<BlipEventArgs<Pickup>> BlipUpdate;

        private Array<Pickup> m_pickups;
        private Array<PhysicalObject> m_objects;
        private Pickup m_activePickup;

        public Array<Pickup> PickupsArray
        {
            get { return m_pickups; }
            set { m_pickups = value; OnPropertyChanged(); }
        }

        public Array<PhysicalObject> ObjectsArray
        {
            get { return m_objects; }
            set { m_objects = value; OnPropertyChanged(); }
        }

        public Pickup ActivePickup
        {
            get { return m_activePickup; }
            set { m_activePickup = value; OnPropertyChanged(); }
        }

        public Pickup NextAvailableSlot
        {
            get
            {
                if (PickupsArray == null) return null;
                return PickupsArray.FirstOrDefault(x => x.ObjectIndex != 0);
            }
        }

        public Pickups(Main mainViewModel)
            : base("Pickups", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        public override void Load()
        {
            base.Load();

            PickupsArray = MainViewModel.TheSave.Pickups.Pickups;
            ObjectsArray = MainViewModel.TheSave.Objects.Objects;

            PickupsArray.CollectionChanged += PickupsArray_CollectionChanged;
            PickupsArray.ItemStateChanged += PickupsArray_ItemStateChanged;
            DrawAllBlips();
        }

        public override void Unload()
        {
            base.Unload();

            RemoveAllBlips();
            PickupsArray.ItemStateChanged -= PickupsArray_ItemStateChanged;
            PickupsArray.CollectionChanged -= PickupsArray_CollectionChanged;

            PickupsArray = null;
            ObjectsArray = null;
        }

        //public void DeleteBlip(Pickup blip)
        //{
        //    blip.Invalidate();
        //}

        private void UpdateBlip(Pickup newBlip)
        {
            BlipUpdate?.Invoke(this, new BlipEventArgs<Pickup>()
            {
                Action = BlipAction.Update,
                Items = { newBlip }
            });
        }

        public void DrawAllBlips()
        {
            BlipUpdate?.Invoke(this, new BlipEventArgs<Pickup>()
            {
                Action = BlipAction.Update,
                Items = PickupsArray
            });
        }

        public void RemoveAllBlips()
        {
            BlipUpdate?.Invoke(this, new BlipEventArgs<Pickup>()
            {
                Action = BlipAction.Reset
            });
        }

        //private short GenerateNewIndex()
        //{
        //    short index;
        //    do
        //    {
        //        index = (short) m_rand.Next(1, short.MaxValue);
        //    } while (Pickups.Any(x => x.Index == index));

        //    return index;
        //}

        private void PickupsArray_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                {
                    BlipUpdate?.Invoke(this, new BlipEventArgs<Pickup>()
                    {
                        Action = BlipAction.Update,
                        Items = e.NewItems.Cast<Pickup>().ToList()
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

        private void PickupsArray_ItemStateChanged(object sender, ItemStateChangedEventArgs e)
        {
            UpdateBlip(PickupsArray[e.ItemIndex]);
        }
    }
}
