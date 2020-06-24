﻿using GTA3SaveEditor.Core;
using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Main : ViewModelBase
    {
        public event EventHandler<MessageBoxEventArgs> MessageBoxRequest;
        public event EventHandler<FileDialogEventArgs> FileDialogRequest;
        public event EventHandler<FileDialogEventArgs> FolderDialogRequest;
        public event EventHandler<GxtSelectionEventArgs> GxtSelectionDialogRequest;
        public event EventHandler<TabUpdateEventArgs> TabUpdate;

        private ObservableCollection<TabPageViewModelBase> m_tabs;
        private int m_selectedTabIndex;
        private string m_statusText;

        public SaveEditor TheEditor { get; private set; }
        public Gxt TheText { get; private set; }
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

        public Main()
        {
            Tabs = new ObservableCollection<TabPageViewModelBase>();
            TheEditor = new SaveEditor();
            TheText = new Gxt();

            Tabs.Add(new Welcome(this));
            Tabs.Add(new General(this));
            Tabs.Add(new Player(this));
            Tabs.Add(new MapViewer(this));
            Tabs.Add(new JsonViewer(this));

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

            if (!string.IsNullOrEmpty(TheSettings.GameDirectory))
            {
                // TODO: select based on language
                TheText = Gxt.LoadFromFile(TheSettings.GameDirectory + @"\TEXT\ENGLISH.GXT");
            }
            
            RefreshTabs(TabUpdateTrigger.WindowLoaded);
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

        #region Window Actions
        public void ShowMessageBoxInfo(string text, string title = "Information")
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Information));
        }

        public void ShowMessageBoxWarning(string text, string title = "Warning")
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Warning));
        }

        public void ShowMessageBoxError(string text, string title = "Error")
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Error));
        }

        public void ShowMessageBoxException(Exception e, string text = "An error has occurred.", string title = "Error")
        {
            text += $"\n\n{e.GetType().Name}: {e.Message}";
            ShowMessageBoxError(text, title);
        }

        public void ShowFileDialog(FileDialogType type, Action<bool?, FileDialogEventArgs> callback)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, callback)
            {
                InitialDirectory = TheSettings.MostRecentFile
            };
            FileDialogRequest?.Invoke(this, e);
        }

        public void ShowFolderDialog(FileDialogType type, Action<bool?, FileDialogEventArgs> callback)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, callback)
            {
                InitialDirectory = TheSettings.MostRecentFile
            };
            FolderDialogRequest?.Invoke(this, e);
        }

        public void ShowGxtSelectionDialog(Action<bool?, GxtSelectionEventArgs> callback)
        {
            GxtSelectionEventArgs e = new GxtSelectionEventArgs()
            {
                GxtTable = TheText,
                Callback = callback
            };
            GxtSelectionDialogRequest?.Invoke(this, e);
        }

        public void RefreshTabs(TabUpdateTrigger trigger)
        {
            TabUpdate?.Invoke(this, new TabUpdateEventArgs(trigger));
            SelectedTabIndex = Tabs.IndexOf(Tabs.Where(x => x.IsVisible).FirstOrDefault());
        }
        #endregion

        #region Window Event Handlers
        private void TheEditor_FileOpened(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TheSave));
            RefreshTabs(TabUpdateTrigger.FileOpened);
            StatusText = "File opened: " + TheSettings.MostRecentFile;
        }

        private void TheEditor_FileClosed(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TheSave));
            RefreshTabs(TabUpdateTrigger.FileClosed);
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
