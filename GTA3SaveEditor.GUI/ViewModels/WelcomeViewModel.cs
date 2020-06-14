using GTA3SaveEditor.Core;
using GTA3SaveEditor.GUI.Events;
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
        private GTA3Save m_selectedFile;
        private ObservableCollection<GTA3Save> m_saveFiles;
        private BackgroundWorker m_lukeFileWalker;

        public GTA3Save SelectedFile
        {
            get { return m_selectedFile; }
            set { m_selectedFile = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GTA3Save> SaveFiles
        {
            get { return m_saveFiles; }
            set { m_saveFiles = value; OnPropertyChanged(); }
        }

        public WelcomeViewModel(MainViewModel mainViewModel)
            : base("Welcome", TabPageVisibility.WhenFileIsClosed, mainViewModel)
        {
            SaveFiles = new ObservableCollection<GTA3Save>();
            MainViewModel.TheSettings.WelcomePath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/GTA3 User Files");
            MainViewModel.TabRefresh += MainViewModel_TabRefresh;
            
            m_lukeFileWalker = new BackgroundWorker();
            m_lukeFileWalker.DoWork += FileWalker_DoWork;
            m_lukeFileWalker.ProgressChanged += FileWalker_ProgressChanged;
            m_lukeFileWalker.RunWorkerCompleted += FileWalker_RunWorkerCompleted;
            m_lukeFileWalker.WorkerSupportsCancellation = true;
            m_lukeFileWalker.WorkerReportsProgress = true;
        }

        public void Refresh()
        {
            SaveFiles.Clear();
            m_lukeFileWalker.CancelAsync();

            if (MainViewModel.TheSettings.WelcomePath == null)
            {
                return;
            }

            m_lukeFileWalker.RunWorkerAsync();
        }

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
                    () => MainViewModel.TheEditor.ActiveFile = SelectedFile,
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
                    () => MainViewModel.RequestFolderDialog(FileDialogType.OpenFileDialog, FolderDialogRequested_Callback)
                );
            }
        }

        private void MainViewModel_TabRefresh(object sender, TabRefreshEventArgs e)
        {
            switch (e.Trigger)
            {
                case TabRefreshTrigger.WindowLoaded:
                case TabRefreshTrigger.FileClosed:
                    Refresh();
                    break;
                case TabRefreshTrigger.FileOpened:
                    m_lukeFileWalker.CancelAsync();
                    break;
            }
        }

        public void FolderDialogRequested_Callback(bool? result, FileDialogEventArgs e)
        {
            if (result == true)
            {
                MainViewModel.TheSettings.WelcomePath = e.FileName;
                Refresh();
            }
        }

        private void FileWalker_DoWork(object sender, DoWorkEventArgs e)
        {
            string folderPath = MainViewModel.TheSettings.WelcomePath;
            bool recurse = MainViewModel.TheSettings.WelcomePathRecurse;

            SearchOption o = (recurse) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath, "*.*", o);

            foreach (string path in files)
            {
                if (SaveEditor.TryOpenFile(path, out GTA3Save saveFile))
                {
                    if (m_lukeFileWalker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    m_lukeFileWalker.ReportProgress(-1, saveFile);
                }
            }
        }

        private void FileWalker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SaveFiles.Add(e.UserState as GTA3Save);
        }

        private void FileWalker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Re-throw any exception that occured, preserving stack trace
                ExceptionDispatchInfo.Capture(e.Error).Throw();
            }
        }
    }
}
