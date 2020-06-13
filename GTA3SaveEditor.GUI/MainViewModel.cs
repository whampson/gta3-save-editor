using GTA3SaveEditor.Core;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Input;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI
{
    public class MainViewModel : ObservableObject
    {
        public event EventHandler<FileDialogEventArgs> FileDialogRequested;
        public event EventHandler<MessageBoxEventArgs> MessageBoxRequested;
        public event EventHandler<TabRefreshEventArgs> TabRefresh;

        private ObservableCollection<TabPageViewModelBase> m_tabs;
        private int m_selectedTabIndex;
        private string m_statusText;

        public SaveEditor TheEditor { get; }
        public GTA3Save TheSave => TheEditor.GetActiveFile();
        public Settings TheSettings => TheEditor.GetSettings();

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
            TheEditor = new SaveEditor();
            Tabs = new ObservableCollection<TabPageViewModelBase>();
        }

        public void Initialize()
        {
            TheEditor.FileOpened += TheEditor_FileOpened;
            TheEditor.FileClosed += TheEditor_FileClosed;
            TheEditor.FileSaved += TheEditor_FileSaved;
            PopulateTabs();
        }

        private void TheEditor_FileOpened(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TheSave));
            OnTabRefresh(TabRefreshTrigger.FileLoaded);
            StatusText = "File opened: " + TheSettings.MostRecentFile;
        }

        private void TheEditor_FileClosed(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TheSave));
            OnTabRefresh(TabRefreshTrigger.FileClosed);
            StatusText = "File closed.";
        }

        private void TheEditor_FileSaved(object sender, EventArgs e)
        {
            StatusText = "File saved: " + TheSettings.MostRecentFile;
        }

        private void PopulateTabs()
        {
            Tabs.Add(new JsonViewModel(this));
            OnTabRefresh(TabRefreshTrigger.WindowLoaded);
        }

        private void OpenFile(string path)
        {
            if (TheEditor.IsFileOpen)
            {
                // TODO: save/discard message
                MessageBoxError("Please close the current file before opening a new one.");
                return;
            }

            try
            {
                TheEditor.OpenFile(path);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                if (e is IOException ||
                    e is SecurityException ||
                    e is UnauthorizedAccessException ||
                    e is SerializationException)
                {
                    MessageBoxException(e, "The file could not be loaded.");
                }
                else if (e is InvalidDataException)
                {
                    MessageBoxError("The file is not a valid GTA3 save.");
                }
            }
        }

        private void SaveFile(string path)
        {
            try
            {
                TheEditor.SaveFile(path);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                if (e is IOException ||
                    e is SecurityException ||
                    e is UnauthorizedAccessException ||
                    e is SerializationException)
                {
                    MessageBoxException(e, "The file could not be saved.");
                }
            }
        }

        private void RequestFileDialog(FileDialogType type)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, FileDialogRequested_Callback)
            {
                InitialDirectory = TheSettings.MostRecentFile,
            };
            FileDialogRequested?.Invoke(this, e);
        }

        private void FileDialogRequested_Callback(bool? result, FileDialogEventArgs e)
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

        private void MessageBoxInfo(string text, string title = "Information")
        {
            MessageBoxRequested?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Information));
        }

        private void MessageBoxWarning(string text, string title = "Warning")
        {
            MessageBoxRequested?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Warning));
        }

        private void MessageBoxError(string text, string title = "Error")
        {
            MessageBoxRequested?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Error));
        }

        private void MessageBoxException(Exception e, string text = "An error has occurred.", string title = "Error")
        {
            string theText = text;
            if (!string.IsNullOrEmpty(theText))
            {
                theText += $"\n\n" +
                    $"{e.GetType().Name}: {e.Message} ({e.HResult})";
            }

            MessageBoxError(theText, title);
        }


        //private void OnPopulateFileTypeList()
        //{
        //    PopulateFileTypeList?.Invoke(this, new FileTypeListEventArgs(FileFormats));
        //}

        private void OnTabRefresh(TabRefreshTrigger trigger, int desiredTabIndex = 0)
        {
            TabRefresh?.Invoke(this, new TabRefreshEventArgs(trigger));

            //if (desiredTabIndex != -1 && desiredTabIndex == SelectedTabIndex)
            //{
            //    SelectedTabIndex = -1;
            //}
            SelectedTabIndex = desiredTabIndex;
        }

        public ICommand FileOpenCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => RequestFileDialog(FileDialogType.OpenFileDialog)
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
                    () => RequestFileDialog(FileDialogType.SaveFileDialog),
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
    }
}
