﻿using GTA3SaveEditor.Core;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Welcome : TabPageViewModelBase
    {
        private readonly BackgroundWorker m_lukeFileWalker;
        private readonly ObservableCollection<ListItem> m_listItems;
        private ListItem m_selectedItem;
        private bool m_openedOnce;

        public string SelectedDirectory
        {
            get { return MainWindow.TheSettings.WelcomePath; }
            set { MainWindow.TheSettings.WelcomePath = value; OnPropertyChanged(); }
        }

        public bool SearchSubDirectories
        {
            get { return MainWindow.TheSettings.WelcomePathRecursiveSearch; }
            set { MainWindow.TheSettings.WelcomePathRecursiveSearch = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ListItem> ListItems
        {
            get { return m_listItems; }
        }

        public ListItem SelectedItem
        {
            get { return m_selectedItem; }
            set { m_selectedItem = value; OnPropertyChanged(); }
        }

        public Welcome(Main mainViewModel)
            : base("Welcome", TabPageVisibility.WhenFileIsClosed, mainViewModel)
        {
            m_listItems = new ObservableCollection<ListItem>();
            m_lukeFileWalker = new BackgroundWorker();
            m_lukeFileWalker.DoWork += FileWalker_DoWork;
            m_lukeFileWalker.ProgressChanged += FileWalker_ProgressChanged;
            m_lukeFileWalker.RunWorkerCompleted += FileWalker_RunWorkerCompleted;
            m_lukeFileWalker.WorkerSupportsCancellation = true;
            m_lukeFileWalker.WorkerReportsProgress = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            

            if (!m_openedOnce)
            {
                if (string.IsNullOrEmpty(SelectedDirectory))
                {
                    string myDocumets = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    SelectedDirectory = Path.GetFullPath(myDocumets + "/GTA3 User Files");
                }
                RefreshList();
                m_openedOnce = true;
            }
            OnPropertyChanged(nameof(SelectedDirectory));
            OnPropertyChanged(nameof(SearchSubDirectories));
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            m_lukeFileWalker.CancelAsync();
        }

        public void RefreshList()
        {
            ListItems.Clear();
            m_lukeFileWalker.CancelAsync();

            if (Directory.Exists(SelectedDirectory))
            {
                m_lukeFileWalker.RunWorkerAsync();
            }
        }

        public void Load()
        {
            if (SelectedItem != null)
            {
                MainWindow.TheSettings.AddRecentFile(SelectedItem.Path);
                MainWindow.TheEditor.ActiveFile = SelectedItem.SaveFile;
            }
        }

        #region Event Handlers
        public void FolderDialogRequested_Callback(bool? result, FileDialogEventArgs e)
        {
            if (result == true)
            {
                SelectedDirectory = e.FileName;
                RefreshList();
            }
        }

        private void FileWalker_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchOption o = (SearchSubDirectories) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(SelectedDirectory, "*.*", o);

            string currDir;
            string lastDir = null;

            foreach (string path in files)
            {
                currDir = Path.GetDirectoryName(path);
                if (currDir != lastDir)
                {
                    lastDir = currDir;
                    MainWindow.StatusText = $"Searching {currDir}...";   // TODO: path shortening func
                }
                if (SaveEditor.TryOpenFile(path, out GTA3Save saveFile))
                {
                    if (m_lukeFileWalker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    ListItem item = new ListItem()
                    {
                        SaveFile = saveFile,
                        Path = path,
                        FileType = saveFile.FileFormat,
                        LastModified = saveFile.TimeStamp,
                        Title = saveFile.Name
                    };

                    if (item.Title.StartsWith('\uFFFF'))
                    {
                        string key = item.Title.Substring(1);
                        if (MainWindow.TheText.TryGetValue(key, out string title))
                        {
                            item.Title = title;
                        }
                    }

                    if (string.IsNullOrEmpty(item.Title))
                    {
                        item.Title = Path.GetFileNameWithoutExtension(path);
                    }
                    
                    if (item.LastModified == DateTime.MinValue)
                    {
                        item.LastModified = File.GetLastWriteTime(path);
                        item.SaveFile.TimeStamp = item.LastModified;
                    }

                    m_lukeFileWalker.ReportProgress(-1, item);
                }
            }
        }

        private void FileWalker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ListItem item)
            {
                ListItems.Add(item);
            }
        }

        private void FileWalker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Re-throw any exception that occured, preserving stack trace
                ExceptionDispatchInfo.Capture(e.Error).Throw();
            }

            MainWindow.StatusText = "Search complete.";
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => RefreshList()
                );
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => Load(),
                    () => SelectedItem != null
                );
            }
        }

        public ICommand BrowseCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => MainWindow.ShowFolderDialog(FileDialogType.OpenFileDialog, FolderDialogRequested_Callback)
                );
            }
        }
        #endregion

        public class ListItem
        {
            public GTA3Save SaveFile { get; set; }
            public string Path { get; set; }
            public string Title { get; set; }
            public DateTime LastModified { get; set; }
            public FileFormat FileType { get; set; }
        }
    }
}
