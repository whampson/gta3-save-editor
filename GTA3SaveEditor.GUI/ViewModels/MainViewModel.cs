using GTA3SaveEditor.Core;
using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public event EventHandler<FileDialogEventArgs> FileDialogRequested;
        public event EventHandler<FileDialogEventArgs> FolderDialogRequested;
        public event EventHandler<MessageBoxEventArgs> MessageBoxRequested;
        public event EventHandler<TabRefreshEventArgs> TabRefresh;

        private ObservableCollection<TabPageViewModelBase> m_tabs;
        private int m_selectedTabIndex;
        private string m_statusText;

        public SaveEditor TheEditor { get; }
        public GTA3Save TheSave => TheEditor.ActiveFile;
        public Settings TheSettings => TheEditor.Settings;

        public ObservableCollection<TabPageViewModelBase> Tabs
        {
            get { return m_tabs; }
            set { m_tabs = value; OnPropertyChanged(); }
        }

        public int SelectedTabIndex
        {
            get { return m_selectedTabIndex; }
            set { m_selectedTabIndex = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get { return m_statusText; }
            set { m_statusText = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabPageViewModelBase>();
            TheEditor = new SaveEditor();

            Tabs.Add(new WelcomeViewModel(this));
            Tabs.Add(new GeneralViewModel(this));
            Tabs.Add(new JsonViewModel(this));

            TheEditor.FileOpened += TheEditor_FileOpened;
            TheEditor.FileClosed += TheEditor_FileClosed;
            TheEditor.FileSaved += TheEditor_FileSaved;
        }

        public void Initialize()
        {
            if (File.Exists(App.SettingsPath))
            {
                TheSettings.LoadSettings(App.SettingsPath);
            }
            
            RefreshTabs(TabRefreshTrigger.WindowLoaded);
            StatusText = "Ready.";
        }

        public void Shutdown()
        {
            TheSettings.SaveSettings(App.SettingsPath);
        }

        public void OpenFile(string path)
        {
            if (TheEditor.IsFileOpen)
            {
                // TODO: save/discard message
                ShowMessageBoxError("Please close the current file before opening a new one.");
                return;
            }

            try
            {
                TheEditor.OpenFile(path);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                if (e is SerializationException)
                {
                    ShowMessageBoxException(e, "The file could not be loaded.");
                    return;
                }
                else if (e is InvalidDataException)
                {
                    ShowMessageBoxError("The file is not a valid GTA3 save file.");
                    return;
                }

                throw;
            }
        }

        public void SaveFile(string path)
        {
            try
            {
                TheEditor.SaveFile(path);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                if (e is SerializationException)
                {
                    ShowMessageBoxException(e, "The file could not be saved.");
                    return;
                }

                throw;
            }
        }

        public void ShowFolderDialog(FileDialogType type, Action<bool?, FileDialogEventArgs> callback)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, callback)
            {
                InitialDirectory = TheSettings.MostRecentFile
            };
            FolderDialogRequested?.Invoke(this, e);
        }

        public void ShowFileDialog(FileDialogType type, Action<bool?, FileDialogEventArgs> callback)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, callback)
            {
                InitialDirectory = TheSettings.MostRecentFile
            };
            FileDialogRequested?.Invoke(this, e);
        }

        public void ShowMessageBoxInfo(string text, string title = "Information")
        {
            MessageBoxRequested?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Information));
        }

        public void ShowMessageBoxWarning(string text, string title = "Warning")
        {
            MessageBoxRequested?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Warning));
        }

        public void ShowMessageBoxError(string text, string title = "Error")
        {
            MessageBoxRequested?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Error));
        }

        public void ShowMessageBoxException(Exception e, string text = "An error has occurred.", string title = "Error")
        {
            text += $"\n\n{e.GetType().Name}: {e.Message}";
            ShowMessageBoxError(text, title);
        }

        public void RefreshTabs(TabRefreshTrigger trigger)
        {
            TabRefresh?.Invoke(this, new TabRefreshEventArgs(trigger));
            SelectedTabIndex = Tabs.IndexOf(Tabs.Where(x => x.IsVisible).FirstOrDefault());
        }

        #region Event Handlers
        private void TheEditor_FileOpened(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TheSave));
            RefreshTabs(TabRefreshTrigger.FileOpened);
            StatusText = "File opened: " + TheSettings.MostRecentFile;
        }

        private void TheEditor_FileClosed(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TheSave));
            RefreshTabs(TabRefreshTrigger.FileClosed);
            StatusText = "File closed.";
        }

        private void TheEditor_FileSaved(object sender, EventArgs e)
        {
            StatusText = "File saved: " + TheSettings.MostRecentFile;
        }

        private void ShowFileDialog_Callback(bool? result, FileDialogEventArgs e)
        {
            if (result != true)
            {
                return;
            }

            TheSettings.LastDirectoryBrowsed = Path.GetDirectoryName(Path.GetFullPath(e.FileName));
            switch (e.DialogType)
            {
                case FileDialogType.OpenFileDialog:
                    OpenFile(e.FileName);
                    break;
                case FileDialogType.SaveFileDialog:
                    SaveFile(e.FileName);
                    break;
            }
        }
        #endregion

        #region Commands
        public ICommand FileOpenCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => ShowFileDialog(FileDialogType.OpenFileDialog, ShowFileDialog_Callback)
                );
            }
        }

        public ICommand FileOpenRecentCommand
        {
            get
            {
                return new RelayCommand<string>
                (
                    (path) => OpenFile(path),
                    (_) => TheSettings.RecentFiles.Count > 0
                );
            }
        }

        public ICommand FileCloseCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => TheEditor.CloseFile(),
                    () => TheEditor.IsFileOpen
                );
            }
        }

        public ICommand FileSaveCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => SaveFile(TheSettings.MostRecentFile),
                    () => TheEditor.IsFileOpen
                );
            }
        }

        public ICommand FileSaveAsCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => ShowFileDialog(FileDialogType.SaveFileDialog, ShowFileDialog_Callback),
                    () => TheEditor.IsFileOpen
                );
            }
        }

        public ICommand FileExitCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => Application.Current.Shutdown()
                );
            }
        }
        #endregion
    }
}
