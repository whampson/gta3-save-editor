using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GTASaveData;
using GTASaveData.GTA3;
using Newtonsoft.Json;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public class Settings : ObservableObject
    {
        public static readonly Settings Defaults = new Settings();

        private int m_recentFilesCapacity;
        private ObservableCollection<string> m_recentFiles;
        private ObservableCollection<string> m_saveFileList;
        //private string m_lastDirAccessed;
        //private string m_lastFileAccessed;
        private bool m_updateTimestamp;
        private bool m_writeLogFile;
        private string m_logFilePath;
        private string m_saveFileDirectory;
        private bool m_recursiveSearch;
        private FileFormatType m_fileFormatOverride;
        private UpdaterSettings m_updater;

        public int RecentFilesCapacity
        {
            get { return m_recentFilesCapacity; }
            set { m_recentFilesCapacity = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> RecentFiles
        {
            get { return m_recentFiles; }
            set { m_recentFiles = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> SaveFileList
        {
            get { return m_saveFileList; }
            set { m_saveFileList = value; OnPropertyChanged(); }
        }

        //public string LastDirectoryAccessed
        //{
        //    get { return m_lastDirAccessed; }
        //    set { m_lastDirAccessed = value; OnPropertyChanged(); }
        //}

        //public string LastFileAccessed
        //{
        //    get { return m_lastFileAccessed; }
        //    set { m_lastFileAccessed = value; OnPropertyChanged(); }
        //}

        public bool UpdateTimeStamp
        {
            get { return m_updateTimestamp; }
            set { m_updateTimestamp = value; OnPropertyChanged(); }
        }

        public bool WriteLogFile
        {
            get { return m_writeLogFile; }
            set { m_writeLogFile = value; OnPropertyChanged(); }
        }

        public string LogFilePath
        {
            get { return m_logFilePath; }
            set { m_logFilePath = value; OnPropertyChanged(); }
        }

        public string SaveFileDirectory
        {
            get { return m_saveFileDirectory; }
            set { m_saveFileDirectory = value; OnPropertyChanged(); }
        }

        public bool RecursiveSearch
        {
            get { return m_recursiveSearch; }
            set { m_recursiveSearch = value; OnPropertyChanged(); }
        }

        public FileFormatType FormatOverride
        {
            get { return m_fileFormatOverride; }
            set { m_fileFormatOverride = value; OnPropertyChanged(); }
        }

        public UpdaterSettings Updater
        {
            get { return m_updater; }
            set { m_updater = value; OnPropertyChanged(); }
        }

        [JsonIgnore] public bool HasFormatOverride => FormatOverride != FileFormatType.None;
        [JsonIgnore] public string MostRecentFile => RecentFiles.FirstOrDefault();

        public Settings()
        {
            RecentFiles = new ObservableCollection<string>();
            SaveFileList = new ObservableCollection<string>();
            RecentFilesCapacity = 10;
            UpdateTimeStamp = true;
            FormatOverride = FileFormatType.None;
            Updater = new UpdaterSettings();
        }

        public void AddRecentFile(string path)
        {
            int index = RecentFiles.IndexOf(path);
            if (index == 0)
            {
                return;
            }
            if (index != -1)
            {
                RecentFiles.Move(index, 0);
                return;
            }

            RecentFiles.Insert(0, path);
            while (RecentFiles.Count > RecentFilesCapacity)
            {
                RecentFiles.RemoveAt(RecentFilesCapacity);
            }
        }

        public void ClearRecentFiles()
        {
            RecentFiles.Clear();
        }

        //public void SetLastAccess(string path)
        //{
        //    bool isDir = Directory.Exists(path);
        //    bool isFile = File.Exists(path);

        //    if (isDir)
        //    {
        //        LastDirectoryAccessed = Path.GetFullPath(path);
        //    }
        //    if (isFile)
        //    {
        //        LastFileAccessed = Path.GetFullPath(path);
        //        LastDirectoryAccessed = Path.GetDirectoryName(path);
        //    }
        //}

        public FileType GetFormatOverride()
        {
            return FormatOverride switch
            {
                FileFormatType.Android => GTA3Save.FileTypes.Android,
                FileFormatType.iOS => GTA3Save.FileTypes.iOS,
                FileFormatType.PC => GTA3Save.FileTypes.PC,
                FileFormatType.PS2 => GTA3Save.FileTypes.PS2,
                FileFormatType.PS2AU => GTA3Save.FileTypes.PS2AU,
                FileFormatType.PS2JP => GTA3Save.FileTypes.PS2JP,
                FileFormatType.Xbox => GTA3Save.FileTypes.Xbox,
                _ => throw new InvalidOperationException("Format override not set.")
            };
        }

        //public bool ShouldSerializeLastDirectoryAccessed() => LastDirectoryAccessed != null;
        //public bool ShouldSerializeLastFileAccessed() => LastFileAccessed != null;
        public bool ShouldSerializeWriteLogFile() => WriteLogFile == true;
        public bool ShouldSerializeLogFilePath() => LogFilePath != null;
        public bool ShouldSerializeFormatOverride() => FormatOverride != FileFormatType.None;

        public class UpdaterSettings : ObservableObject
        {
            private bool m_checkForUpdatesAtStartup;
            private bool m_preReleaseRing;

            public bool CheckForUpdatesAtStartup
            {
                get { return m_checkForUpdatesAtStartup; }
                set { m_checkForUpdatesAtStartup = value; OnPropertyChanged(); }
            }

            public bool PreReleaseRing
            {
                get { return m_preReleaseRing; }
                set { m_preReleaseRing = value; OnPropertyChanged(); }
            }

            public UpdaterSettings()
            {
                CheckForUpdatesAtStartup = true;
            }

            public bool ShouldSerializePreReleaseRing() => PreReleaseRing == true;
        }
    }
}
