using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Extensions;
using GTASaveData;
using GTASaveData.GTA3;
using WpfEssentials;

namespace GTA3SaveEditor.GUI
{
    public class SaveFileInfo : ObservableObject
    {
        private GTA3Save m_saveFile;
        private string m_path;
        private string m_title;
        private DateTime m_lastModified;
        private FileType m_fileType;

        public GTA3Save SaveFile
        {
            get { return m_saveFile; }
            set { m_saveFile = value; OnPropertyChanged(); }
        }

        public string FilePath
        {
            get { return m_path; }
            set { m_path = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return m_title; }
            set { m_title = value; OnPropertyChanged(); }
        }

        public DateTime LastModified
        {
            get { return m_lastModified; }
            set { m_lastModified = value; OnPropertyChanged(); }
        }

        public FileType FileType
        {
            get { return m_fileType; }
            set { m_fileType = value; OnPropertyChanged(); }
        }

        public static bool TryGetInfo(string path, out SaveFileInfo info)
        {
            if (!SaveEditor.TryLoadFile(path, out GTA3Save saveFile))
            {
                info = new SaveFileInfo()
                {
                    FilePath = path,
                    FileType = FileType.Default,
                    LastModified = DateTime.MinValue,
                    Title = "(invalid save file)"
                };
                return false;
            }

            info = new SaveFileInfo()
            {
                FilePath = path,
                FileType = saveFile.GetFileType(),
                LastModified = saveFile.TimeStamp,
                Title = saveFile.GetTitle()
            };

            saveFile.Dispose();
            return true;
        }
    }
}
