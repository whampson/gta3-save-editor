using System;
using System.IO;
using GTASaveData;
using GTASaveData.GTA3;
using WpfEssentials;

namespace GTA3SaveEditor.Core
{
    public partial class SaveEditor : ObservableObject
    {
        public event EventHandler<string> FileOpening;
        public event EventHandler FileOpened;
        public event EventHandler<string> FileSaving;
        public event EventHandler FileSaved;
        public event EventHandler FileClosing;
        public event EventHandler FileClosed;

        public string ActiveFilePath => Settings.MostRecentFile;
        public bool IsEditingFile => ActiveFile != null;

        private SaveFileGTA3 m_activeFile;
        private DateTime m_lastWriteTime;

        public SaveFileGTA3 ActiveFile
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

        public void OpenFile(string path)
        {
            if (IsEditingFile) throw FileAlreadyOpened();

            OnFileOpening(path);

            var format = Settings.HasFormatOverride ? Settings.GetFormatOverride() : FileFormat.Default;
            ActiveFile = SaveFileGTA3.Load(path, format) ?? throw BadSaveData();
            LastWriteTime = File.GetLastWriteTime(path);
            Settings.AddRecentFile(path);

            OnPropertyChanged(nameof(IsEditingFile));
            OnFileOpened();
        }

        public void SaveFile()
        {
            SaveFile(ActiveFilePath);
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

            OnPropertyChanged(nameof(IsEditingFile));
            OnFileClosed();
        }

        private void OnFileOpening(string path) => FileOpening?.Invoke(this, path);
        private void OnFileOpened() => FileOpened?.Invoke(this, EventArgs.Empty);
        private void OnFileClosing() => FileClosing?.Invoke(this, EventArgs.Empty);
        private void OnFileClosed() => FileClosed?.Invoke(this, EventArgs.Empty);
        private void OnFileSaving(string path) => FileSaving?.Invoke(this, path);
        private void OnFileSaved() => FileSaved?.Invoke(this, EventArgs.Empty);

        private InvalidDataException BadSaveData() => throw new InvalidDataException("Not a valid GTA3 save file!");
        private InvalidOperationException FileAlreadyOpened() => new InvalidOperationException("A file is already open!");
        private InvalidOperationException NoFileOpen() => new InvalidOperationException("No file is open!");
}
}
