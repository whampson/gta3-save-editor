using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.IO;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public class SaveEditor : ObservableObject
    {
        public event EventHandler FileOpened;
        public event EventHandler FileSaved;
        public event EventHandler FileClosed;

        private Settings m_settings;
        private GTA3Save m_theSave;

        public bool IsFileOpen => m_theSave != null;

        public SaveEditor()
        {
            m_settings = new Settings();
        }

        public Settings GetSettings()
        {
            return m_settings;
        }

        public GTA3Save GetActiveFile()
        {
            return m_theSave;
        }

        public void OpenFile(string path)
        {
            if (IsFileOpen)
            {
                throw FileAlreadyLoaded();
            }

            m_theSave = ((m_settings.AutoDetectFileType)
                ? SaveData.Load<GTA3Save>(path)
                : SaveData.Load<GTA3Save>(path, m_settings.ForcedFileType))
                ?? throw BadSaveData();

            m_settings.AddRecentFile(path);
            OnFileOpened();
        }

        public void SaveFile(string path)
        {
            if (!IsFileOpen)
            {
                throw NoFileLoaded();
            }

            if (m_settings.UpdateTimeStampOnSave)
            {
                m_theSave.TimeStamp = DateTime.Now;
            }

            m_theSave.Save(path);
            m_settings.AddRecentFile(path);
            OnFileSaved();
        }

        public void CloseFile()
        {
            if (!IsFileOpen)
            {
                throw NoFileLoaded();
            }

            m_theSave.Dispose();
            m_theSave = null;

            OnFileClosed();
        }

        private void OnFileOpened()
        {
            
            OnPropertyChanged(nameof(IsFileOpen));
            FileOpened?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileSaved()
        {
            FileSaved?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileClosed()
        {
            OnPropertyChanged(nameof(IsFileOpen));
            FileClosed?.Invoke(this, EventArgs.Empty);
        }

        private InvalidOperationException FileAlreadyLoaded()
        {
            return new InvalidOperationException("File already loaded!");
        }

        private InvalidOperationException NoFileLoaded()
        {
            return new InvalidOperationException("No file loaded!");
        }

        private InvalidDataException BadSaveData()
        {
            throw new InvalidDataException("Not a valid GTA3 save file!");
        }
    }
}
