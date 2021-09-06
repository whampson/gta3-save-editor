using WpfEssentials;

namespace GTA3SaveEditor.GUI
{
    public class SaveSlot : ObservableObject
    {
        private string m_name;
        private string m_path;
        private bool m_inUse;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged(); }
        }

        public string Path
        {
            get { return m_path; }
            set { m_path = value; OnPropertyChanged(); }
        }

        public bool InUse
        {
            get { return m_inUse; }
            set { m_inUse = value; OnPropertyChanged(); }
        }
    }
}
