using GTA3SaveEditor.Core;
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
    public class Welcome : BaseTabPage
    {
        private readonly BackgroundWorker m_lukeFileWalker;
        private readonly ObservableCollection<ListItem> m_listItems;
        private ListItem m_selectedItem;
        private bool m_openedOnce;

        public string SelectedDirectory
        {
            get { return MainViewModel.TheSettings.WelcomePath; }
            set { MainViewModel.TheSettings.WelcomePath = value; OnPropertyChanged(); }
        }

        public bool SearchSubDirectories
        {
            get { return MainViewModel.TheSettings.WelcomePathRecursiveSearch; }
            set { MainViewModel.TheSettings.WelcomePathRecursiveSearch = value; OnPropertyChanged(); }
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
            m_lukeFileWalker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
        }

        public override void Initialize()
        {
            base.Initialize();
            m_lukeFileWalker.DoWork += FileWalker_DoWork;
            m_lukeFileWalker.ProgressChanged += FileWalker_ProgressChanged;
            m_lukeFileWalker.RunWorkerCompleted += FileWalker_RunWorkerCompleted;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_lukeFileWalker.DoWork -= FileWalker_DoWork;
            m_lukeFileWalker.ProgressChanged -= FileWalker_ProgressChanged;
            m_lukeFileWalker.RunWorkerCompleted -= FileWalker_RunWorkerCompleted;
        }

        public override void Load()
        {
            base.Load();
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

        public override void Unload()
        {
            base.Unload();
            m_lukeFileWalker.CancelAsync();
        }

        public override void Update()
        {
            base.Update();
            RefreshList();
        }

        public void RefreshList()
        {
            // TODO: detect external changes
            ListItems.Clear();
            m_lukeFileWalker.CancelAsync();
            if (Directory.Exists(SelectedDirectory))
            {
                m_lukeFileWalker.RunWorkerAsync();
            }
        }

        public void LoadSelectedItem()
        {
            if (SelectedItem != null)
            {
                MainViewModel.TheSettings.AddRecentFile(SelectedItem.Path);
                MainViewModel.TheEditor.ActiveFile = SelectedItem.SaveFile;
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
                    MainViewModel.StatusText = $"Searching {currDir}...";   // TODO: path shortening func
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
                        if (MainViewModel.TheText.TryGetValue(key, out string title))
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
                // TODO: catch some exceptions: eg access denied
                ExceptionDispatchInfo.Capture(e.Error).Throw();
            }

            MainViewModel.TimedStatusTextTime = 5;
            MainViewModel.TimedStatusText = "Search complete.";
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand =>  new RelayCommand
        (
            () => RefreshList()
        );

        public ICommand LoadCommand => new RelayCommand
        (
            () => LoadSelectedItem(),
            () => SelectedItem != null
        );

        public ICommand BrowseCommand => new RelayCommand
        (
            () => MainViewModel.ShowFolderDialog(FileDialogType.OpenFileDialog, FolderDialogRequested_Callback)
        );
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
