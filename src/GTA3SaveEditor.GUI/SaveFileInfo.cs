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
        private SaveFileGTA3 m_saveFile;
        private string m_path;
        private string m_title;
        private DateTime m_lastModified;
        private FileFormat m_fileType;

        public SaveFileGTA3 SaveFile
        {
            get { return m_saveFile; }
            set { m_saveFile = value; OnPropertyChanged(); }
        }

        public string Path
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

        public FileFormat FileType
        {
            get { return m_fileType; }
            set { m_fileType = value; OnPropertyChanged(); }
        }

        public static bool TryGetInfo(string path, out SaveFileInfo info)
        {
            if (SaveEditor.TryLoadFile(path, out SaveFileGTA3 saveFile))
            {
                DateTime timeStamp = (saveFile.FileFormat.IsPC || saveFile.FileFormat.IsXbox)
                    ? saveFile.SimpleVars.TimeStamp
                    : File.GetLastWriteTime(path);

                info = new SaveFileInfo()
                {
                    Path = path,
                    FileType = saveFile.FileFormat,
                    LastModified = timeStamp,
                    Title = saveFile.GetSaveName()
                };

                saveFile.Dispose();
                return true;
            }

            info = new SaveFileInfo()
            {
                Path = path,
                FileType = FileFormat.Default,
                LastModified = DateTime.MinValue,
                Title = "(invalid save file)"
            };

            return false;
        }
    }
}
