using GTASaveData;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public class Settings : ObservableObject
    {
        public const int DefaultRecentFilesCapacity = 10;

        [JsonProperty(PropertyName = "RecentFiles")]
        private ObservableCollection<string> m_recentFiles;

        private int m_recentFilesCapacity;
        private string m_lastDirectoryBrowsed;
        private string m_welcomePath;
        private bool m_welcomePathRecurse;
        private bool m_updateTimeStamp;
        private bool m_autoDetectFileType;
        private FileFormat m_forcedFileType;

        [JsonIgnore]
        public ReadOnlyObservableCollection<string> RecentFiles
        {
            get { return new ReadOnlyObservableCollection<string>(m_recentFiles); }
        }

        [JsonIgnore]
        public string MostRecentFile
        {
            get { return (m_recentFiles.Count > 0) ? m_recentFiles[0] : null; }
        }

        public int RecentFilesCapacity
        {
            get { return m_recentFilesCapacity; }
            set { m_recentFilesCapacity = value; OnPropertyChanged(); }
        }

        public string LastDirectoryBrowsed
        {
            get { return m_lastDirectoryBrowsed; }
            set { m_lastDirectoryBrowsed = value; OnPropertyChanged(); }
        }

        public string WelcomePath
        {
            get { return m_welcomePath; }
            set { m_welcomePath = value; OnPropertyChanged(); }
        }

        public bool WelcomePathRecurse
        {
            get { return m_welcomePathRecurse; }
            set { m_welcomePathRecurse = value; OnPropertyChanged(); }
        }

        public bool UpdateTimeStampOnSave
        {
            get { return m_updateTimeStamp; }
            set { m_updateTimeStamp = value; OnPropertyChanged(); }
        }

        public bool AutoDetectFileType
        {
            get { return m_autoDetectFileType; }
            set { m_autoDetectFileType = value; OnPropertyChanged(); }
        }

        public FileFormat ForcedFileType
        {
            get { return m_forcedFileType; }
            set { m_forcedFileType = value; OnPropertyChanged(); }
        }

        public Settings()
        {
            m_updateTimeStamp = true;
            m_autoDetectFileType = true;
            m_recentFiles = new ObservableCollection<string>();
            m_recentFilesCapacity = DefaultRecentFilesCapacity;
        }

        public void AddRecentFile(string path)
        {
            int index = m_recentFiles.IndexOf(path);
            if (index == 0)
            {
                return;
            }
            if (index != -1)
            {
                m_recentFiles.Move(index, 0);
                return;
            }

            m_recentFiles.Insert(0, path);
            while (m_recentFiles.Count > m_recentFilesCapacity)
            {
                m_recentFiles.RemoveAt(m_recentFilesCapacity);
            }
        }

        public void ClearRecentFiles()
        {
            m_recentFiles.Clear();
        }

        public void LoadSettings(string path)
        {
            string settingsJson = File.ReadAllText(path);
            JsonConvert.PopulateObject(settingsJson, this);
        }

        public void SaveSettings(string path)
        {
            string settingsJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, settingsJson);
        }
    }
}
