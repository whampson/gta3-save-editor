using GTA3SaveEditor.Core;
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
    public class WelcomeViewModel : TabPageViewModelBase
    {
        private readonly BackgroundWorker m_lukeFileWalker;
        private readonly ObservableCollection<GTA3Save> m_saveFiles;
        private readonly Dictionary<GTA3Save, string> m_saveFilePaths;
        private GTA3Save m_selectedFile;

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

        public ObservableCollection<GTA3Save> SaveFiles
        {
            get { return m_saveFiles; }
        }

        public Dictionary<GTA3Save, string> SaveFilePaths
        {
            get { return m_saveFilePaths; }
        }

        public GTA3Save SelectedFile
        {
            get { return m_selectedFile; }
            set { m_selectedFile = value; OnPropertyChanged(); }
        }

        public WelcomeViewModel(MainViewModel mainViewModel)
            : base("Welcome", TabPageVisibility.WhenFileIsClosed, mainViewModel)
        {
            m_saveFilePaths = new Dictionary<GTA3Save, string>();
            m_saveFiles = new ObservableCollection<GTA3Save>();
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
            if (string.IsNullOrEmpty(SelectedDirectory))
            {
                string myDocumets = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                SelectedDirectory = Path.GetFullPath(myDocumets + "/GTA3 User Files");
            }

            Refresh();
            OnPropertyChanged(nameof(SelectedDirectory));
            OnPropertyChanged(nameof(SearchSubDirectories));
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            m_lukeFileWalker.CancelAsync();
        }

        public void Refresh()
        {
            SaveFiles.Clear();
            m_saveFilePaths.Clear();
            m_lukeFileWalker.CancelAsync();

            if (Directory.Exists(SelectedDirectory))
            {
                m_lukeFileWalker.RunWorkerAsync();
            }
        }

        public void Load()
        {
            if (SelectedFile != null)
            {
                MainViewModel.TheSettings.AddRecentFile(m_saveFilePaths[SelectedFile]);
                MainViewModel.TheEditor.ActiveFile = SelectedFile;
            }
        }

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => Refresh()
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
                    () => SelectedFile != null
                );
            }
        }

        public ICommand BrowseCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => MainViewModel.ShowFolderDialog(FileDialogType.OpenFileDialog, FolderDialogRequested_Callback)
                );
            }
        }
        #endregion

        #region Event Handlers
        public void FolderDialogRequested_Callback(bool? result, FileDialogEventArgs e)
        {
            if (result == true)
            {
                SelectedDirectory = e.FileName;
                Refresh();
            }
        }

        private void FileWalker_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchOption o = (SearchSubDirectories) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(SelectedDirectory, "*.*", o);

            foreach (string path in files)
            {
                if (SaveEditor.TryOpenFile(path, out GTA3Save saveFile))
                {
                    if (m_lukeFileWalker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    m_lukeFileWalker.ReportProgress(-1, new Tuple<GTA3Save, string>(saveFile, path));
                }
            }
        }

        private void FileWalker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var data = e.UserState as Tuple<GTA3Save, string>;
            var saveFile = data.Item1;
            var path = data.Item2;

            SaveFiles.Add(saveFile);
            SaveFilePaths[saveFile] = path;
        }

        private void FileWalker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Re-throw any exception that occured, preserving stack trace
                ExceptionDispatchInfo.Capture(e.Error).Throw();
            }
        }
        #endregion
    }
}
