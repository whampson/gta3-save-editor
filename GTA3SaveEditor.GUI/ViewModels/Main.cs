using GTA3SaveEditor.Core;
using GTA3SaveEditor.GUI.Events;
using GTASaveData;
using GTASaveData.GTA3;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public class Main : ViewModelBase
    {
        public const string ProgramTitle = "Grand Theft Auto III Save Editor";

        public event EventHandler<MessageBoxEventArgs> MessageBoxRequest;
        public event EventHandler<FileDialogEventArgs> FileDialogRequest;
        public event EventHandler<FileDialogEventArgs> FolderDialogRequest;
        public event EventHandler<GxtSelectionEventArgs> GxtSelectionDialogRequest;
        public event EventHandler<TabUpdateEventArgs> TabUpdate;

        private readonly DispatcherTimer m_statusTextTimer;
        private ObservableCollection<TabPageViewModelBase> m_tabs;
        private int m_selectedTabIndex;
        private string m_title;
        private string m_statusText;
        private string m_defaultStatusText;
        private string m_timedStatusText;
        private int m_timedStatusTextTime;
        private int m_timedStatusTextTick;
        private bool m_isRefreshingFile;
        private bool m_isDirty;

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

        public string Title
        {
            get { return m_title; }
            set { m_title = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get { return m_statusText; }
            set { m_statusText = value; OnPropertyChanged(); }
        }

        public string DefaultStatusText
        {
            get { return m_defaultStatusText; }
            set { m_defaultStatusText = value; OnPropertyChanged(); }
        }

        public string TimedStatusText
        {
            get { return m_timedStatusText; }
            set
            {
                m_timedStatusText = value;
                m_timedStatusTextTick = TimedStatusTextTime;
                m_statusTextTimer.Interval = TimeSpan.Zero;
                m_statusTextTimer.Start();
                OnPropertyChanged();
            }
        }

        public int TimedStatusTextTime
        {
            get { return m_timedStatusTextTime; }
            set { m_timedStatusTextTime = value; OnPropertyChanged(); }
        }

        public bool IsDirty
        {
            get { return m_isDirty; }
            set { m_isDirty = value; OnPropertyChanged(); }
        }

        public Main()
        {
            Tabs = new ObservableCollection<TabPageViewModelBase>();
            TheEditor = new SaveEditor();
            TheText = new Gxt();

            Tabs.Add(new Welcome(this));
            Tabs.Add(new General(this));
            Tabs.Add(new Player(this));
            Tabs.Add(new Objects(this));
            Tabs.Add(new Pickups(this));
            Tabs.Add(new Radar(this));
            Tabs.Add(new JsonViewer(this));

            m_statusTextTimer = new DispatcherTimer();

            UpdateTitle();
        }

        public void Initialize()
        {
            TheEditor.FileOpening += TheEditor_FileOpening;
            TheEditor.FileOpened += TheEditor_FileOpened;
            TheEditor.FileClosing += TheEditor_FileClosing;
            TheEditor.FileClosed += TheEditor_FileClosed;
            TheEditor.FileSaving += TheEditor_FileSaving;
            TheEditor.FileSaved += TheEditor_FileSaved;
            m_statusTextTimer.Tick += StatusTimer_Tick;

            LoadSettings();
            InitializeTabs();
            RefreshTabs(TabUpdateTrigger.WindowLoaded);

            DefaultStatusText = "Ready.";
            StatusText = DefaultStatusText;
        }

        public void Shutdown()
        {
            TheEditor.FileOpening -= TheEditor_FileOpening;
            TheEditor.FileOpened -= TheEditor_FileOpened;
            TheEditor.FileClosing -= TheEditor_FileClosing;
            TheEditor.FileClosed -= TheEditor_FileClosed;
            TheEditor.FileSaving -= TheEditor_FileSaving;
            TheEditor.FileSaved -= TheEditor_FileSaved;
            m_statusTextTimer.Tick -= StatusTimer_Tick;

            RefreshTabs(TabUpdateTrigger.WindowClosing);
            ShutdownTabs();
            SaveSettings();
        }

        private void InitializeTabs()
        {
            foreach (var tab in Tabs)
            {
                tab.Initialize();
            }
        }

        private void ShutdownTabs()
        {
            foreach (var tab in Tabs)
            {
                tab.Shutdown();
            }
        }

        public void LoadSettings()
        {
            // Load settings file
            if (File.Exists(App.SettingsPath))
            {
                TheSettings.LoadSettings(App.SettingsPath);
            }

            // Initialize settings-dependent features
            if (!string.IsNullOrEmpty(TheSettings.GameDirectory))
            {
                // TODO: select based on language
                TheText = Gxt.LoadFromFile(TheSettings.GameDirectory + @"\TEXT\ENGLISH.GXT");
            }
        }

        public void SaveSettings()
        {
            TheSettings.SaveSettings(App.SettingsPath);
        }

        public void OpenFile(string path)
        {
            if (TheEditor.IsFileOpen)
            {
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

        public void SaveFile()
        {
            SaveFile(TheSettings.MostRecentFile);
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

        public void RefreshFile()
        {
            if (IsDirty) ShowSaveConfirmationDialog(RefreshFileDialog_Callback);
            else DoRefresh();
        }

        public void DoRefresh()
        {
            m_isRefreshingFile = true;

            Tabs.Where(t => t.Visibility == TabPageVisibility.WhenFileIsOpen).ToList()
                .ForEach(t => t.Unload());

            TheEditor.CloseFile();
            TheEditor.OpenFile(TheSettings.MostRecentFile);

            Tabs.Where(t => t.Visibility == TabPageVisibility.WhenFileIsOpen).ToList()
                .ForEach(t => t.Load());

            m_isRefreshingFile = false;
        }

        public void CloseFile()
        {
            if (IsDirty) ShowSaveConfirmationDialog(CloseFileDialog_Callback);
            else DoClose();
        }

        private void DoClose()
        {
            TheEditor.CloseFile();
        }

        private void DoExit()
        {
            Application.Current.Shutdown();
        }

        private void UpdateTitle()
        {
            string title = ProgramTitle;
            if (IsDirty) title = $"*{title}";
            if (TheSave != null) title += $" - {TheSettings.MostRecentFile}";

            Title = title;
        }

        private void SetDirty()
        {
            if (!IsDirty)
            {
                IsDirty = true;
                UpdateTitle();
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

        public void ShowSaveConfirmationDialog(Action<MessageBoxResult> callback)
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                "Do you want to save your changes?", "Save", 
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question, callback: callback));
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
        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            if (m_timedStatusTextTick == TimedStatusTextTime)
            {
                m_statusTextTimer.Interval = TimeSpan.FromSeconds(1);
            }
            if (m_timedStatusTextTick <= 0)
            {
                m_statusTextTimer.Stop();
                StatusText = DefaultStatusText;
            }
            else
            {
                StatusText = TimedStatusText;
                m_timedStatusTextTick--;
            }
        }

        private void TheEditor_FileOpening(object sender, EventArgs e)
        {
            if (!m_isRefreshingFile)
            {
                StatusText = "Opening file...";
            }
        }

        private void TheEditor_FileOpened(object sender, EventArgs e)
        {
            UpdateTitle();

            RegisterDirtyHandlers(TheSave);
            OnPropertyChanged(nameof(TheSave));

            if (!m_isRefreshingFile)
            {
                RefreshTabs(TabUpdateTrigger.FileOpened);
                TimedStatusTextTime = 5;
                TimedStatusText = "File opened successfully.";
            }
            else
            {
                TimedStatusTextTime = 5;
                TimedStatusText = "File refreshed.";
            }
        }

        private void TheEditor_FileClosing(object sender, EventArgs e)
        {
            if (!m_isRefreshingFile)
            {
                StatusText = "Closing file...";
                RefreshTabs(TabUpdateTrigger.FileClosing);
            }
        }

        private void TheEditor_FileClosed(object sender, EventArgs e)
        {
            IsDirty = false;
            UpdateTitle();

            UnregisterDirtyHandlers(TheSave);
            OnPropertyChanged(nameof(TheSave));
            TimedStatusTextTime = 5;
            TimedStatusText = "File closed.";
        }

        private void TheEditor_FileSaving(object sender, EventArgs e)
        {
            StatusText = "Saving file...";
            if (TheSettings.UpdateTimeStampOnSave)
            {
                TheSave.TimeStamp = DateTime.Now;
            }
        }

        private void TheEditor_FileSaved(object sender, EventArgs e)
        {
            IsDirty = false;
            UpdateTitle();
            TimedStatusTextTime = 5;
            TimedStatusText = "File saved successfully.";
        }

        void RefreshFileDialog_Callback(MessageBoxResult r)
        {
            if (r != MessageBoxResult.Cancel)
            {
                if (r == MessageBoxResult.Yes) SaveFile();
                DoRefresh();
            }
        }

        void CloseFileDialog_Callback(MessageBoxResult r)
        {
            if (r != MessageBoxResult.Cancel)
            {
                if (r == MessageBoxResult.Yes) SaveFile();
                DoClose();
            }
        }

        private void ShowFileDialog_Callback(bool? result, FileDialogEventArgs e)
        {
            if (result != true) return;

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

        private void RegisterDirtyHandlers(INotifyPropertyChanged o)
        {
            if (o == null) return;

            foreach (var p in o.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0))
            {
                object v = p.GetValue(o, null);
                if (p.PropertyType.GetInterface(nameof(INotifyItemStateChanged)) != null)
                {
                    (v as INotifyItemStateChanged).ItemStateChanged += TheSave_CollectionElementDirty;
                }
                if (p.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) != null)
                {
                    (v as INotifyCollectionChanged).CollectionChanged += TheSave_CollectionDirty;
                }
                if (p.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
                {
                    RegisterDirtyHandlers(v as INotifyPropertyChanged);
                }
            }
            o.PropertyChanged += TheSave_PropertyDirty;
        }

        private void UnregisterDirtyHandlers(INotifyPropertyChanged o)
        {
            if (o == null) return;

            foreach (var p in o.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0))
            {
                object v = p.GetValue(o, null);
                if (p.PropertyType.GetInterface(nameof(INotifyItemStateChanged)) != null)
                {
                    (v as INotifyItemStateChanged).ItemStateChanged -= TheSave_CollectionElementDirty;
                }
                if (p.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) != null)
                {
                    (v as INotifyCollectionChanged).CollectionChanged -= TheSave_CollectionDirty;
                }
                if (p.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
                {
                    RegisterDirtyHandlers(v as INotifyPropertyChanged);
                }
            }
            o.PropertyChanged -= TheSave_PropertyDirty;
        }

        private void TheSave_PropertyDirty(object sender, PropertyChangedEventArgs e)
        {
            SetDirty();
        }

        private void TheSave_CollectionDirty(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetDirty();
        }

        private void TheSave_CollectionElementDirty(object sender, ItemStateChangedEventArgs e)
        {
            SetDirty();
        }
        #endregion

        #region Commands
        public ICommand FileOpenCommand => new RelayCommand
        (
            () => ShowFileDialog(FileDialogType.OpenFileDialog, ShowFileDialog_Callback)
        );

        public ICommand FileOpenRecentCommand => new RelayCommand<string>
        (
            (x) => OpenFile(x),
            (_) => TheSettings.RecentFiles.Count > 0
        );

        public ICommand FileCloseCommand => new RelayCommand
        (
            () => CloseFile(),
            () => TheEditor.IsFileOpen
        );

        public ICommand FileSaveCommand => new RelayCommand
        (
            () => SaveFile(),
            () => TheEditor.IsFileOpen
        );

        public ICommand FileSaveAsCommand => new RelayCommand
        (
            () => ShowFileDialog(FileDialogType.SaveFileDialog, ShowFileDialog_Callback),
            () => TheEditor.IsFileOpen
        );

        public ICommand FileRefreshCommand => new RelayCommand
        (
            () => RefreshFile(),
            () => TheEditor.IsFileOpen
        );

        public ICommand FileExitCommand => new RelayCommand
        (
            () => Application.Current.MainWindow.Close()
        );
        #endregion
    }
}
