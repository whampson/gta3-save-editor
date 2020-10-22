using System;
using System.IO;
using GTA3SaveEditor.Core.Util;
using GTASaveData;
using GTASaveData.GTA3;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public partial class SaveEditor : ObservableObject
    {
        #region File I/O Events
        public event EventHandler<string> FileOpening;
        public event EventHandler FileOpened;
        public event EventHandler<string> FileSaving;
        public event EventHandler FileSaved;
        public event EventHandler FileClosing;
        public event EventHandler FileClosed;
        #endregion

        public string ActiveFilePath => Settings.MostRecentFile;
        public bool IsEditingFile => ActiveFile != null;

        private GTA3Save m_activeFile;
        private DateTime m_lastWriteTime;

        public GTA3Save ActiveFile
        {
            get { return m_activeFile; }
            private set { m_activeFile = value; OnPropertyChanged(); }
        }

        public DateTime LastWriteTime
        {
            get { return m_lastWriteTime; }
            set { m_lastWriteTime = value; OnPropertyChanged(); }
        }

        private SaveEditor()
        { }

        #region File I/O
        public void OpenFile(string path)
        {
            if (IsEditingFile) throw FileAlreadyOpened();

            OnFileOpening(path);

            var format = Settings.HasFormatOverride ? Settings.GetFormatOverride() : FileFormat.Default;
            ActiveFile = SaveData.Load<GTA3Save>(path, format) ?? throw BadSaveData();
            LastWriteTime = File.GetLastWriteTime(path);

            Settings.AddRecentFile(path);
            OnFileOpened();
        }

        public void SaveFile(string path)
        {
            if (!IsEditingFile) throw NoFileOpen();

            OnFileSaving(path);
            ActiveFile.Save(path);
            LastWriteTime = File.GetLastWriteTime(path);

            Settings.AddRecentFile(path);
            OnFileSaved();
        }

        public void CloseFile()
        {
            if (!IsEditingFile) return;

            OnFileClosing();
            ActiveFile.Dispose();
            ActiveFile = null;
            OnFileClosed();
        }

        private void OnFileOpening(string path)
        {
            Log.InfoF("Opening file{0}...", (path != null) ? $": {path}" : "");
            FileOpening?.Invoke(this, path);
        }

        private void OnFileOpened()
        {
            OnPropertyChanged(nameof(IsEditingFile));
            FileOpened?.Invoke(this, EventArgs.Empty);

            Log.Info("File opened:");
            Log.Info($"        Name = {ActiveFile.Name}");
            Log.Info($"        Type = {ActiveFile.FileFormat}");
            Log.Info($"  Time Stamp = {ActiveFile.TimeStamp}");
            Log.Info($"    Progress = {(ActiveFile.Stats.ProgressMade / ActiveFile.Stats.TotalProgressInGame):P2}");
            Log.Info($"Last Mission = {ActiveFile.Stats.LastMissionPassedName}");
            Log.Info($"   MAIN Size = {ActiveFile.Scripts.MainScriptSize}");
            Log.Info($"     Threads = {ActiveFile.Scripts.Threads.Count}");
        }

        private void OnFileClosing()
        {
            Log.Info("Closing file...");
            FileClosing?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileClosed()
        {
            OnPropertyChanged(nameof(IsEditingFile));
            FileClosed?.Invoke(this, EventArgs.Empty);
            Log.Info("File closed.");
        }

        private void OnFileSaving(string path)
        {
            Log.InfoF($"Saving file: {path}...");
            FileSaving?.Invoke(this, path);
        }

        private void OnFileSaved()
        {
            Log.Info("File saved.");
            FileSaved?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Exceptions
        private InvalidDataException BadSaveData()
        {
            throw new InvalidDataException("Not a valid GTA3 save file!");
        }

        private InvalidOperationException FileAlreadyOpened()
        {
            return new InvalidOperationException("A file is already open!");
        }

        private InvalidOperationException NoFileOpen()
        {
            return new InvalidOperationException("No file is open!");
        }
        #endregion

    }
}
