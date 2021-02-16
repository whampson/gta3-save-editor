using System.Collections.Generic;
using System.Linq;
using GTA3SaveEditor.Core.Game;
using GTASaveData;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI.Tabs
{
    public class PickupsTabVM : TabPageVM
    {
        public static List<string> ObjectModels { get; }

        static PickupsTabVM()
        {
            ObjectModels = GTA3.IdeObjects
                .Select(o => o.ModelName)
                .ToList();
        }


        public Array<Pickup> m_pickups;
        public Pickup m_selectedPickup;

        public Array<Pickup> Pickups
        {
            get { return m_pickups; }
            set { m_pickups = value; OnPropertyChanged(); }
        }

        public Pickup SelectedPickup
        {
            get { return m_selectedPickup; }
            set { m_selectedPickup = value; OnPropertyChanged(); }
        }

        public override void Load()
        {
            base.Load();
            Pickups = TheSave.Pickups.Pickups;
        }
    }
}
