using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.IO;
using System.Security;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public class SaveEditor : ObservableObject
    {
        public event EventHandler FileOpened;
        public event EventHandler FileSaved;
        public event EventHandler FileClosed;

        private Settings m_settings;
        private GTA3Save m_activeFile;

        public bool IsFileOpen => m_activeFile != null;

        public Settings Settings
        {
            get { return m_settings; }
            set { m_settings = value; OnPropertyChanged(); }
        }

        public GTA3Save ActiveFile
        {
            get { return m_activeFile; } 
            set {
                CloseFile();

                m_activeFile = value;
                if (m_activeFile != null)
                {
                    OnFileOpened();
                }
                OnPropertyChanged();
            }
        }

        public SaveEditor()
        {
            Settings = new Settings();
            ActiveFile = null;
        }

        public void OpenFile(string path)
        {
            if (IsFileOpen)
            {
                throw FileAlreadyLoaded();
            }

            ActiveFile = ((Settings.AutoDetectFileType)
                ? SaveData.Load<GTA3Save>(path)
                : SaveData.Load<GTA3Save>(path, Settings.ForcedFileType))
                ?? throw BadSaveData();

            Settings.AddRecentFile(path);
            OnFileOpened();
        }



        public void SaveFile(string path)
        {
            if (!IsFileOpen)
            {
                throw NoFileLoaded();
            }

            if (Settings.UpdateTimeStampOnSave)
            {
                ActiveFile.TimeStamp = DateTime.Now;
            }

            ActiveFile.Save(path);
            Settings.AddRecentFile(path);
            OnFileSaved();
        }

        public void CloseFile()
        {
            if (IsFileOpen)
            {
                m_activeFile.Dispose();
                m_activeFile = null;
                OnFileClosed();
                OnPropertyChanged(nameof(ActiveFile));
            }
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

        public static bool TryOpenFile(string path, out GTA3Save saveFile)
        {
            try
            {
                saveFile = SaveData.Load<GTA3Save>(path);
                return saveFile != null;
            }
            catch (Exception e)
            {
                if (e is IOException ||
                    e is SecurityException ||
                    e is UnauthorizedAccessException ||
                    e is SerializationException ||
                    e is InvalidDataException)
                {
                    Log.Exception(e);
                    saveFile = null;
                    return false;
                }
                throw;
            }
        }
    }
}
