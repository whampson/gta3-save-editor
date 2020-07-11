using GTASaveData;
using GTASaveData.GTA3;
using GTASaveData.Types;
using System;
using System.IO;
using System.Security;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public class SaveEditor : ObservableObject
    {
        public event EventHandler FileOpening;
        public event EventHandler FileOpened;
        public event EventHandler FileClosing;
        public event EventHandler FileClosed;
        public event EventHandler FileSaving;
        public event EventHandler FileSaved;

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
                if (value != null) OnFileOpening();
                m_activeFile = value;
                if (value != null) OnFileOpened();
                OnPropertyChanged();
            }
        }

        public SaveEditor()
        {
            Settings = new Settings();
            ActiveFile = null;
        }

        public void SetPlayerSpawnPoint(Vector3D loc)
        {
            ActiveFile.PlayerPeds.GetPlayerPed().Position = loc;

            // Reset save threads so new spawn location isn't overridden
            // TOOD: offsets for other SCM versions (this is v2)
            RunningScript iSave = ActiveFile.Scripts.GetRunningScript("i_save");
            RunningScript cSave = ActiveFile.Scripts.GetRunningScript("c_save");
            RunningScript sSave = ActiveFile.Scripts.GetRunningScript("s_save");
            if (iSave != null) iSave.IP = 60371;
            if (cSave != null) cSave.IP = 62138;
            if (sSave != null) sSave.IP = 63554;
        } 

        public void OpenFile(string path)
        {
            if (IsFileOpen)
            {
                throw FileAlreadyLoaded();
            }

            OnFileOpening();
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

            OnFileSaving();
            ActiveFile.Save(path);
            Settings.AddRecentFile(path);
            OnFileSaved();
        }

        public void CloseFile()
        {
            if (IsFileOpen)
            {
                OnFileClosing();
                m_activeFile.Dispose();
                m_activeFile = null;
                OnFileClosed();
                OnPropertyChanged(nameof(ActiveFile));
            }
        }

        private void OnFileOpening()
        {
            FileOpening?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileOpened()
        {
            OnPropertyChanged(nameof(IsFileOpen));
            FileOpened?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileClosing()
        {
            FileClosing?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileClosed()
        {
            OnPropertyChanged(nameof(IsFileOpen));
            FileClosed?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileSaving()
        {
            FileSaving?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileSaved()
        {
            FileSaved?.Invoke(this, EventArgs.Empty);
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
