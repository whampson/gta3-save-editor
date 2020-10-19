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
        private string m_lastDirAccessed;
        private string m_lastFileAccessed;
        private bool m_writeSaveTimestamp;
        private bool m_writeLogFile;
        private string m_logFilePath;
        private FileFormatType m_fileFormatOverride;
        //private UpdaterSettings m_updater;

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

        [JsonIgnore]
        public string MostRecentFile => RecentFiles.FirstOrDefault();

        public string LastDirectoryAccessed
        {
            get { return m_lastDirAccessed; }
            set { m_lastDirAccessed = value; OnPropertyChanged(); }
        }

        public string LastFileAccessed
        {
            get { return m_lastFileAccessed; }
            set { m_lastFileAccessed = value; OnPropertyChanged(); }
        }

        public bool WriteSaveTimestamp
        {
            get { return m_writeSaveTimestamp; }
            set { m_writeSaveTimestamp = value; OnPropertyChanged(); }
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

        public FileFormatType FormatOverride
        {
            get { return m_fileFormatOverride; }
            set { m_fileFormatOverride = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public bool HasFormatOverride => FormatOverride != FileFormatType.None;

        public FileFormat GetOverrideFormat()
        {
            return FormatOverride switch
            {
                FileFormatType.Android => GTA3Save.FileFormats.Android,
                FileFormatType.iOS => GTA3Save.FileFormats.iOS,
                FileFormatType.PC => GTA3Save.FileFormats.PC,
                FileFormatType.PS2 => GTA3Save.FileFormats.PS2,
                FileFormatType.PS2AU => GTA3Save.FileFormats.PS2_AU,
                FileFormatType.PS2JP => GTA3Save.FileFormats.PS2_JP,
                FileFormatType.Xbox => GTA3Save.FileFormats.Xbox,
                _ => throw new InvalidOperationException("Format override not set.")
            };
        }

        public Settings()
        {
            RecentFiles = new ObservableCollection<string>();
            RecentFilesCapacity = 10;
            WriteSaveTimestamp = true;
            FormatOverride = FileFormatType.None;
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

        public void SetLastAccess(string path)
        {
            bool isDir = Directory.Exists(path);
            bool isFile = File.Exists(path);

            if (isDir)
            {
                LastDirectoryAccessed = Path.GetFullPath(path);
            }
            if (isFile)
            {
                LastFileAccessed = Path.GetFullPath(path);
                LastDirectoryAccessed = Path.GetDirectoryName(path);
            }
        }

        protected bool ShouldSerializeWriteLogFile() => WriteLogFile == true;
        protected bool ShouldSerializeLogFilePath() => LogFilePath != null;
        protected bool ShouldSerializeFormatOverride() => FormatOverride != FileFormatType.None;
    }
}
