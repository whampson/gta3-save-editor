using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Util;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Tabs
{
    public class WelcomeTabVM : TabPageVM
    {
        const int GameQuoteInterval = 5;

        private readonly DispatcherTimer m_gameQuoteTimer;
        private BackgroundWorker m_lukeFileWalker;
        private BackgroundWorker m_fileListWorker;
        private ObservableCollection<SaveFileInfo> m_saveFiles;
        private SaveFileInfo m_selectedFile;
        private bool m_isSearching;
        private bool m_isSearchPending;
        private bool m_isCancelPendingForSearch;
        private bool m_openedOnce;

        private static string AppendTrailingSlash(string path)
        {
            if (path != null && !path.EndsWith('\\'))
            {
                path += "\\";
            }

            return path;
        }

        public string SelectedDirectory
        {
            get { return AppendTrailingSlash(Settings.SaveFileDirectory); }
            set { Settings.SaveFileDirectory = AppendTrailingSlash(value); OnPropertyChanged(); }
        }

        public bool RecursiveSearch
        {
            get { return Settings.RecursiveSearch; }
            set { Settings.RecursiveSearch = value; OnPropertyChanged(); }
        }

        public BackgroundWorker SearchWorker
        {
            get { return m_lukeFileWalker; }
            set { m_lukeFileWalker = value; OnPropertyChanged(); }
        }

        public BackgroundWorker FileListWorker
        {
            get { return m_fileListWorker; }
            set { m_fileListWorker = value; OnPropertyChanged(); }
        }

        public ObservableCollection<SaveFileInfo> SaveFiles
        {
            get { return m_saveFiles; }
            private set { m_saveFiles = value; OnPropertyChanged(); }
        }

        public SaveFileInfo SelectedFile
        {
            get { return m_selectedFile; }
            set { m_selectedFile = value; OnPropertyChanged(); }
        }

        public bool IsSearchPending
        {
            get { return m_isSearchPending; }
            set { m_isSearchPending = value; OnPropertyChanged(); }
        }

        public bool IsCancelPending
        {
            get { return m_isCancelPendingForSearch; }
            set { m_isCancelPendingForSearch = value; OnPropertyChanged(); }
        }

        public bool IsSearching
        {
            get { return m_isSearching; }
            set { m_isSearching = value; OnPropertyChanged(); }
        }

        public WelcomeTabVM()
        {
            m_gameQuoteTimer = new DispatcherTimer();
            SaveFiles = new ObservableCollection<SaveFileInfo>();
            SearchWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            FileListWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
        }

        public override void Init()
        {
            base.Init();
            SearchWorker.DoWork += LukeFileWalker_DoWork;
            SearchWorker.ProgressChanged += LukeFileWalker_ProgressChanged;
            SearchWorker.RunWorkerCompleted += LukeFileWalker_RunWorkerCompleted;
            FileListWorker.DoWork += FileListWorker_DoWork;
            FileListWorker.ProgressChanged += FileListWorker_ProgressChanged;
            FileListWorker.RunWorkerCompleted += FileListWorker_RunWorkerCompleted;
            m_gameQuoteTimer.Tick += GameQuoteTimer_Tick;
            m_gameQuoteTimer.Interval = TimeSpan.FromSeconds(GameQuoteInterval);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            SearchWorker.DoWork -= LukeFileWalker_DoWork;
            SearchWorker.ProgressChanged -= LukeFileWalker_ProgressChanged;
            SearchWorker.RunWorkerCompleted -= LukeFileWalker_RunWorkerCompleted;
            FileListWorker.DoWork -= FileListWorker_DoWork;
            FileListWorker.ProgressChanged -= FileListWorker_ProgressChanged;
            FileListWorker.RunWorkerCompleted -= FileListWorker_RunWorkerCompleted;
            m_gameQuoteTimer.Tick -= GameQuoteTimer_Tick;
        }

        public override void Load()
        {
            base.Load();

            if (string.IsNullOrEmpty(SelectedDirectory))
            {
                SelectedDirectory = SaveEditor.DefaultSaveFileDirectory;
            }

            if (SaveFiles.Count == 0 && Settings.SaveFileList.Count > 0)
            {
                PopulateSaveFileList();
            }
            else if (!m_openedOnce)
            {
                SearchForSaveFiles();   // Only search on program launch
            }

            m_gameQuoteTimer.Start();
            m_openedOnce = true;
        }

        public override void Unload()
        {
            base.Unload();
            CancelSearch();

            m_gameQuoteTimer.Stop();
        }

        public override void Update()
        {
            base.Update();
            ShowGameQuote();
            //RefreshSaveFileList();
        }

        public void RefreshSaveFileList()
        {
            CancelPopulateList();
            PopulateSaveFileList();
        }

        public void SearchForSaveFiles()
        {
            if (IsSearching)
            {
                IsSearchPending = true;
                CancelSearch();
                return;
            }

            IsSearchPending = false;
            if (Directory.Exists(SelectedDirectory))
            {
                Settings.SaveFileList.Clear();
                SaveFiles.Clear();

                SearchWorker.RunWorkerAsync();
                IsSearching = true;

                Log.Info($"Searching for GTA3 save files...{(RecursiveSearch ? " (recursive)" : "")}");
            }
        }

        public void CancelSearch()
        {
            if (!IsCancelPending)
            {
                SearchWorker.CancelAsync();
                IsCancelPending = true;
            }
        }

        public void PopulateSaveFileList()
        {
            FileListWorker.RunWorkerAsync();
        }

        public void CancelPopulateList()
        {
            FileListWorker.CancelAsync();
        }

        public void OpenSelectedItem()
        {
            CancelSearch();

            if (SelectedFile != null)
            {
                Editor.OpenFile(SelectedFile.Path);
            }
        }

        public void ShowGameQuote()
        {
            Random r = new Random();
            int quoteIndex = r.Next(0, GameQuotes.Count);
            TheWindow.SetStatusText(GameQuotes[quoteIndex]);
        }

        public static readonly List<string> GameQuotes = new List<string>()
        {
            // creepy guy
            "My mother's my sister!",
            "What? She's my cousin?!",
            //"I swear I thought she was my second cousin.",

            // mafia
            "Real good red sauce, like blood!",

            // hooker
            "Hey hey, you wanna party?",

            // dock worker
            "You can sail the seven seas!",
            "In the Navy!",

            // triads
            "You feeling lucky punk?",
            "I see pain in your future!",

            // colombians
            "You want the chainsaw, gringo?",
            "It's no problem to kill you!",
        };

        private void GameQuoteTimer_Tick(object sender, EventArgs e)
        {
            ShowGameQuote();
        }

        #region Event Handlers
        private void LukeFileWalker_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchOption o = (RecursiveSearch) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(SelectedDirectory, "*.*", o);

            string currDir;
            string lastDir = null;

            foreach (string path in files)
            {
                if (SearchWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                currDir = Path.GetDirectoryName(path);
                if (currDir != lastDir)
                {
                    lastDir = currDir;
                    if (!m_openedOnce)
                    {
                        TheWindow.SetStatusText(@$"Searching {currDir}\...");   // TODO: path shortening func
                    }
                }

                if (SaveFileInfo.TryGetInfo(path, out SaveFileInfo info))
                {
                    SearchWorker.ReportProgress(-1, info);
                }
            }
        }

        private void LukeFileWalker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is SaveFileInfo info)
            {
                Settings.SaveFileList.Add(info.Path);
                SaveFiles.Add(info);
            }
        }

        private void LukeFileWalker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsSearching = false;
            IsCancelPending = false;

            if (e.Cancelled)
            {
                Log.Info("Search cancelled.");
                TheWindow.SetTimedStatusText("Search cancelled.");
                return;
            }

            if (e.Error != null)
            {
                Log.Exception(e.Error);

                // Filter exceptions.
                if (e.Error is UnauthorizedAccessException)
                {
                    return;
                }

                DebugHelper.CaptureAndThrow(e.Error);

                Log.Info($"Search completed with errors. Found {SaveFiles.Count} save files.");
                if (!m_openedOnce)
                {
                    TheWindow.SetTimedStatusText("Search completed with errors. See the log for details.");
                }
                return;
            }

            if (IsSearchPending)
            {
                SearchForSaveFiles();
                return;
            }

            Log.Info($"Search completed. Found {SaveFiles.Count} save files.");
            if (!m_openedOnce)
            {
                TheWindow.SetTimedStatusText("Search completed.");
            }
        }

        private void FileListWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> toRemove = new List<string>();
            foreach (string path in Settings.SaveFileList)
            {
                if (!SaveFileInfo.TryGetInfo(path, out SaveFileInfo info))
                {
                    toRemove.Add(path);
                    continue;
                }
                FileListWorker.ReportProgress(-1, info);
            }

            if (toRemove.Count > 0)
            {
                FileListWorker.ReportProgress(-1, toRemove);
            }
        }

        private void FileListWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is SaveFileInfo info)
            {
                SaveFiles.Add(info);
            }
            else if (e.UserState is List<string> toRemove)
            {
                foreach (string path in toRemove)
                {
                    Settings.SaveFileList.Remove(path);
                }
            }
        }

        private void FileListWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Exception(e.Error);
                DebugHelper.CaptureAndThrow(e.Error);
            }
        }
        #endregion

        #region Commands
        public ICommand SearchToggleCommand => new RelayCommand
        (
            () => { if (!IsSearching) SearchForSaveFiles(); else CancelSearch(); }
        );

        public ICommand OpenCommand => new RelayCommand
        (
            () => OpenSelectedItem(),
            () => SelectedFile != null
        );

        public ICommand BrowseCommand => new RelayCommand
        (
            () => TheWindow.ShowFolderDialog((r,e) =>
            {
                if (r == true)
                {
                    SelectedDirectory = e.FileName;
                    SearchForSaveFiles();
                }
            })
        );

        public ICommand RefreshCommand => new RelayCommand
        (
            () =>
            {
                RefreshSaveFileList();
                TheWindow.SetTimedStatusText("File list refreshed.");
            }
        );
        #endregion
    }
}
