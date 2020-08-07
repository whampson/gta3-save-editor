using GTASaveData;
using GTASaveData.GTA3;
using System.Linq;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Objects : BaseTabPage
    {
        private Array<PhysicalObject> m_objects;
        private PhysicalObject m_activeObject;

        public Array<PhysicalObject> ObjectsArray
        {
            get { return m_objects; }
            set { m_objects = value; OnPropertyChanged(); }
        }

        public PhysicalObject ActiveObject
        {
            get { return m_activeObject; }
            set { m_activeObject = value; OnPropertyChanged(); }
        }

        public PhysicalObject NextAvailableSlot
        {
            get
            {
                if (ObjectsArray == null) return null;
                return ObjectsArray.FirstOrDefault(x => x.Handle != 0);
            }
        }

        public Objects(Main mainViewModel)
            : base("Objects", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }

        public override void Load()
        {
            base.Load();
            ObjectsArray = MainViewModel.TheSave.Objects.Objects;
        }
    }
}
