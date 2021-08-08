using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Util;
using GTA3SaveEditor.GUI.Events;
using GTASaveData.GTA3;
using WHampson.ToolUI;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI
{
    public abstract class BaseWindowVM : WindowViewModelBase
    {
        public event EventHandler LogWindowRequest;
        public event EventHandler CustomScriptsDialogRequest;
        public event EventHandler<FileIOEventArgs> OpenFileRequest;
        public event EventHandler<FileIOEventArgs> CloseFileRequest;
        public event EventHandler<FileIOEventArgs> SaveFileRequest;
        public event EventHandler<FileIOEventArgs> RevertFileRequest;
        public event EventHandler<InputDialogEventArgs> InputDialogRequest;

        public SaveEditor Editor => SaveEditor.Instance;
        public Settings Settings => SaveEditor.Settings;
        public SaveFileGTA3 TheSave => SaveEditor.Instance.ActiveFile;

        private bool m_suppressExternalChangesCheck;

        public bool SuppressExternalChangesCheck
        {
            get { return m_suppressExternalChangesCheck; }
            set { m_suppressExternalChangesCheck = value; OnPropertyChanged(); }
        }

        public void CheckForExternalChanges()
        {
            if (!Editor.IsEditingFile || SuppressExternalChangesCheck)
            {
                return;
            }

            DateTime lastWriteTime = File.GetLastWriteTime(Settings.MostRecentFile);
            if (lastWriteTime != Editor.LastWriteTime)
            {
                Log.Info("External changes detected.");
                PromptYesNo(
                    "The file has been modified by another program.\n\n" +
                    "Do you want to reload the file? You will lose all unsaved changes.",
                    title: "External Changes Detected",
                    icon: MessageBoxImage.Exclamation,
                    yesAction: () => RevertFileRequest?.Invoke(this, new FileIOEventArgs() { SuppressDialogs = true }),
                    noAction: () => SuppressExternalChangesCheck = true);
            }
        }

        public void OpenFile(string path) => OpenFileRequest?.Invoke(this, new FileIOEventArgs() { Path = path });
        public void CloseFile() => CloseFileRequest?.Invoke(this, new FileIOEventArgs());
        public void SaveFile() => SaveFile(Editor.ActiveFilePath);
        public void SaveFile(string path) => SaveFileRequest?.Invoke(this, new FileIOEventArgs() { Path = path });
        public void RevertFile() => RevertFileRequest?.Invoke(this, new FileIOEventArgs());

        public ICommand CloseFileCommand => new RelayCommand
        (
            () => CloseFile(),
            () => Editor.IsEditingFile
        );

        public ICommand SaveFileCommand => new RelayCommand
        (
            () =>
            {
                SaveFile();
            },
            () => Editor.IsEditingFile
        );

        public ICommand RevertFileCommand => new RelayCommand
        (
            () => RevertFile(),
            () => Editor.IsEditingFile
        );

        public new void SetStatusText(string status)
        {
            Log.Info($"Status: {status}");
            base.SetStatusText(status);
        }

        public new void SetTimedStatusText(string status,
            double duration = 5,
            string expiredStatus = "Ready.")
        {
            Log.Info($"Status: {status}");
            base.SetTimedStatusText(status, duration, expiredStatus);
        }

        public void ShowLogWindow() => LogWindowRequest?.Invoke(this, EventArgs.Empty);
        public void ShowCustomScriptsDialog() => CustomScriptsDialogRequest?.Invoke(this, EventArgs.Empty);

        public bool ShowInputDialog(string label, out string result)
        {
            bool valid = false;
            string s = null;

            InputDialogRequest?.Invoke(this, new InputDialogEventArgs() {
                Label = label,
                Callback = (e) =>
                {
                    valid = e.Result != null;
                    s = e.Result;
                }
            });

            result = s;
            return valid;
        }
    }
}
