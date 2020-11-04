using System.Collections.Generic;
using System.Linq;
using GTA3SaveEditor.Core;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI
{
    public class PickupsTabVM : TabPageVM
    {
        public static List<string> ObjectModels { get; }

        static PickupsTabVM()
        {
            ObjectModels = SaveEditor.IdeObjects
                .Select(o => o.ModelName)
                .Where(m => !m.StartsWith("LOD"))
                .ToList();
        }

        public Pickup m_selectedPickup;

        public Pickup SelectedPickup
        {
            get { return m_selectedPickup; }
            set { m_selectedPickup = value; OnPropertyChanged(); }
        }
    }
}
