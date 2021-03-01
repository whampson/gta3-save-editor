using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Extensions;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Loaders;
using GTA3SaveEditor.Core.Util;
using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.Tabs;
using GTASaveData;
using GTASaveData.GTA3;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI
{
    public class MainWindowVM : WindowVMBase
    {
        private const int NumSaveSlots = 8;
        private const string FileFilter = "GTA3 Save Files (*.b)|*.b|All Files|*.*";

        const string TabNameWelcome = "Welcome";
        const string TabNameScripts = "Scripts";
        const string TabNameGangs = "Gangs";
        const string TabNamePickups = "Pickups";

        private ObservableCollection<SaveSlot> m_saveSlots;
        private ObservableCollection<TabPageVM> m_tabs;
        private int m_selectedTabIndex;
        private bool m_isDirty;

        public ObservableCollection<SaveSlot> SaveSlots
        {
            get { return m_saveSlots; }
            private set { m_saveSlots = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TabPageVM> Tabs
        {
            get { return m_tabs; }
            private set { m_tabs = value; OnPropertyChanged(); }
        }

        public int SelectedTabIndex
        {
            get { return m_selectedTabIndex; }
            set { m_selectedTabIndex = value; OnPropertyChanged(); }
        }

        public bool IsDirty
        {
            get { return m_isDirty; }
            private set { m_isDirty = value; OnPropertyChanged(); }
        }

        public MainWindowVM()
        {
            var slots = Enumerable.Range(0, NumSaveSlots).Select(slot => new SaveSlot());
            SaveSlots = new ObservableCollection<SaveSlot>(slots);
            Tabs = new ObservableCollection<TabPageVM>()
            {
                new WelcomeTabVM()  { TheWindow = this, Title = TabNameWelcome, Visibility = TabPageVisibility.WhenNotEditingFile },
                new GangsTabVM()    { TheWindow = this, Title = TabNameGangs,   Visibility = TabPageVisibility.WhenEditingFile },
                new PickupsTabVM()  { TheWindow = this, Title = TabNamePickups, Visibility = TabPageVisibility.WhenEditingFile },
                new ScriptsTabVM()  { TheWindow = this, Title = TabNameScripts, Visibility = TabPageVisibility.WhenEditingFile },
            };
        }

        public override void Init()
        {
            base.Init();
            Title = App.Name;

            Editor.FileOpening += FileOpening_Handler;
            Editor.FileOpened += FileOpened_Handler;
            Editor.FileClosing += FileClosing_Handler;
            Editor.FileClosed += FileClosed_Handler;
            Editor.FileSaving += FileSaving_Handler;
            Editor.FileSaved += FileSaved_Handler;

            InitAllTabs();
            RefreshTabVisibility();
            RefreshSaveSlots();
        }

        public override void Shutdown()
        {
            base.Shutdown();

            Editor.FileOpening -= FileOpening_Handler;
            Editor.FileOpened -= FileOpened_Handler;
            Editor.FileClosing -= FileClosing_Handler;
            Editor.FileClosed -= FileClosed_Handler;
            Editor.FileSaving -= FileSaving_Handler;
            Editor.FileSaved -= FileSaved_Handler;

            ShutdownAllTabs();
        }

        public void SetDirty()
        {
            if (!IsDirty)
            {
                IsDirty = true;
                UpdateTitle();
            }
        }

        private void ClearDirty()
        {
            IsDirty = false;
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            string title = App.Name;
            if (Editor.IsEditingFile)
            {
                title += " - " + Editor.ActiveFilePath;
            }
            if (IsDirty)
            {
                title = "*" + title;
            }

            Title = title;
        }

        public T GetTab<T>() where T : TabPageVM
        {
            return (T) Tabs.Where(t => t is T).FirstOrDefault();
        }

        private void InitAllTabs()
        {
            foreach (var tab in Tabs) tab.Init();
        }

        private void ShutdownAllTabs()
        {
            foreach (var tab in Tabs) tab.Shutdown();
        }

        private void UpdateVisibleTabs()
        {
            foreach (var tab in Tabs)
                if (tab.IsVisible) tab.Update();
        }

        private void RefreshTabVisibility()
        {
            foreach (var tab in Tabs)
            {
                tab.IsVisible =
                    (tab.Visibility == TabPageVisibility.Always) ||
                    (tab.Visibility == TabPageVisibility.WhenEditingFile && Editor.IsEditingFile) ||
                    (tab.Visibility == TabPageVisibility.WhenNotEditingFile && !Editor.IsEditingFile);
            }
        }

        private void SelectFirstVisibleTab()
        {
            SelectedTabIndex = Tabs.IndexOf(Tabs.Where(x => x.IsVisible).FirstOrDefault());
        }

        private void RefreshSaveSlots()
        {
            string gta3UserFiles = SaveEditor.DefaultSaveFileDirectory;

            int slotNum = 1;
            foreach (var slot in SaveSlots)
            {
                slot.Path = Path.Combine(gta3UserFiles, $"GTA3sf{slotNum}.b");
                slot.Name = $"(slot is free)";
                slot.InUse = false;

                if (File.Exists(slot.Path))
                {
                    SaveEditor.TryLoadFile(slot.Path, out SaveFileGTA3 save);
                    slot.Name = save?.Name ?? $"(invalid save file)";
                    slot.InUse = true;
                    save?.Dispose();
                }

                slotNum++;
            }

            Log.Info("Loaded PC save slots.");
        }

        public void ExitAppWithConfirmation()
        {
            CloseFileRoutine(() => Application.Current.Shutdown());
        }

        public void OpenFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            if (IsDirty)
            {
                CloseFileRoutine(() => OpenFileRoutine(e.Path), suppressDialogs: e.SuppressDialogs);
                return;
            }

            //Settings.SetLastAccess(e.Path);
            OpenFileRoutine(e.Path, suppressDialogs: e.SuppressDialogs);
        }

        public void CloseFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            CloseFileRoutine(suppressDialogs: e.SuppressDialogs);
        }

        public void SaveFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            try
            {
                // TODO: test save slot when directory doesn't exist
                Editor.SaveFile(e.Path);
            }
            catch (EndOfStreamException)
            {
                ShowError("Error saving file! Block size limit exceeded.\n\n" +
                    "This happens if you tried to add too many things to the save file " +
                    "(e.g Global Variales, Custom Scripts, Threads, Objects, or Vehicles). " +
                    "The game has a limit on how large parts of the save file can be and " +
                    "will crash when attempting to load the save.\n",
                    "Error Saving File");
            }
        }

        public void RevertFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            // TODO
            throw new NotImplementedException();
        }

        private void FileOpening_Handler(object sender, string e)
        {
            SetTimedStatusText("Opening file...");
        }

        private void FileOpened_Handler(object sender, EventArgs e)
        {
            SuppressExternalChangesCheck = false;
            
            RegisterDirtyHandlers(TheSave);
            UpdateTitle();
            RefreshTabVisibility();
            UpdateVisibleTabs();
            SelectFirstVisibleTab();

            OnPropertyChanged(nameof(TheSave));
            SetTimedStatusText("File opened.");

            string lastMissionKey = TheSave.Stats.LastMissionPassedName;
            string lastMission = GTA3.GetGxtString(lastMissionKey);
            Log.Info($"   File Type: {TheSave.FileFormat}");
            Log.Info($"       Title: {TheSave.GetSaveName()}");
            Log.Info($"Last Mission: {lastMission}");
            Log.Info($"  Time Stamp: {TheSave.TimeStamp}");
            Log.Info($"    Progress: {((float) TheSave.Stats.ProgressMade / TheSave.Stats.TotalProgressInGame):P2}");
            Log.Info($"   MAIN Size: {TheSave.Scripts.MainScriptSize}");
            Log.Info($"Num. Globals: {TheSave.Scripts.Globals.Count()}");
            Log.Info($"Num. Threads: {TheSave.Scripts.Threads.Count}");
            Log.Info($"Num. Objects: {TheSave.Objects.Objects.Count}");
        }

        private void FileClosing_Handler(object sender, EventArgs e)
        {
            SetTimedStatusText("Closing file...");
            UnregisterDirtyHandlers(TheSave);
        }

        private void FileClosed_Handler(object sender, EventArgs e)
        {
            SuppressExternalChangesCheck = false;

            ClearDirty();
            UpdateTitle();
            RefreshTabVisibility();
            UpdateVisibleTabs();
            SelectFirstVisibleTab();

            OnPropertyChanged(nameof(TheSave));
            SetTimedStatusText("File closed.", expiredStatus: null);
        }

        private void FileSaving_Handler(object sender, string e)
        {
            SetTimedStatusText("Saving file...");
        }

        private void FileSaved_Handler(object sender, EventArgs e)
        {
            SuppressExternalChangesCheck = false;
            ClearDirty();

            SetTimedStatusText("File saved.");
        }

        private bool OpenFileRoutine(string path, Action onFileOpened = null, bool suppressDialogs = false)
        {
            try
            {
                Editor.OpenFile(path);
                onFileOpened?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                SetTimedStatusText("Error opening file.", 10);

                bool isBadSaveFile = ex is InvalidDataException;
                if (isBadSaveFile && !suppressDialogs)
                {
                    ShowError("The file is not a valid GTA III save file.");
                    return false;
                }
                if (!suppressDialogs)
                {
                    ShowException(ex, "An error occurred while opening the file.");
                }

                DebugHelper.CaptureAndThrow(ex);
                return false;
            }
        }

        private bool CloseFileRoutine(Action onFileClosed = null, bool suppressDialogs = false)
        {
            if (!Editor.IsEditingFile)
            {
                onFileClosed?.Invoke();
                return true;
            }

            if (IsDirty && !suppressDialogs)
            {
                bool retval = false;
                PromptYesNoCancel(
                    "Would you like to save your changes before closing this file?",
                    title: "Save Changes?",
                    yesAction: () =>
                    {
                        Editor.SaveFile();
                        retval = CloseFileRoutine(onFileClosed);
                    },
                    noAction: () =>
                    {
                        ClearDirty();
                        retval = CloseFileRoutine(onFileClosed);
                    },
                    cancelAction: () =>
                    {
                        retval = false;
                    });
                return retval;
            }

            Editor.CloseFile();
            onFileClosed?.Invoke();
            return true;
        }

        // TODO: provide a way for tabs to add/remove handlers from dirty event

        private void RegisterDirtyHandlers(INotifyPropertyChanged o)
        {
            if (o == null) return;

            foreach (var p in o.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0))
            {
                object v = p.GetValue(o, null);
                if (p.PropertyType.GetInterface(nameof(INotifyItemStateChanged)) != null)
                {
                    (v as INotifyItemStateChanged).ItemStateChanged += TheSave_CollectionElementChanged;
                    Log.Debug($"Registered ItemStateChanged handler on {p.PropertyType}");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) != null)
                {
                    (v as INotifyCollectionChanged).CollectionChanged += TheSave_CollectionChanged;
                    Log.Debug($"Registered CollectionChanged handler on {p.PropertyType}");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
                {
                    RegisterDirtyHandlers(v as INotifyPropertyChanged);
                }
            }
            o.PropertyChanged += TheSave_PropertyChanged;
            Log.Debug($"Registered PropertyChanged handler on {o}");
        }

        private void UnregisterDirtyHandlers(INotifyPropertyChanged o)
        {
            if (o == null) return;

            foreach (var p in o.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0))
            {
                object v = p.GetValue(o, null);
                if (p.PropertyType.GetInterface(nameof(INotifyItemStateChanged)) != null)
                {
                    (v as INotifyItemStateChanged).ItemStateChanged -= TheSave_CollectionElementChanged;
                    Log.Debug($"Unregistered ItemStateChanged handler on {p.PropertyType}");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) != null)
                {
                    (v as INotifyCollectionChanged).CollectionChanged -= TheSave_CollectionChanged;
                    Log.Debug($"Unregistered CollectionChanged handler on {p.PropertyType}");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
                {
                    UnregisterDirtyHandlers(v as INotifyPropertyChanged);
                }
            }
            o.PropertyChanged -= TheSave_PropertyChanged;
            Log.Debug($"Unregistered PropertyChanged handler on {o}");
        }

        private void TheSave_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetDirty();
            
            // TODO: log property, invoke Dirty event
        }

        private void TheSave_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetDirty();

            // TODO: log property, invoke Dirty event
        }

        private void TheSave_CollectionElementChanged(object sender, ItemStateChangedEventArgs e)
        {
            SetDirty();

            // TODO: log property, invoke Dirty event
        }

        public ICommand OpenRecentFileCommand => new RelayCommand<string>
        (
            (x) => OpenFile(x),
            (_) => Settings.RecentFiles.Count > 0
        );

        public ICommand OpenFileCommand => new RelayCommand
        (
            () => ShowFileDialog(new FileDialogEventArgs(FileDialogType.OpenFileDialog)
            {
                Filter = FileFilter,
                Callback = (r, e) => { if (r == true) CloseFileRoutine(() => OpenFile(e.FileName)); }
            })
        );

        public ICommand SaveFileAsCommand => new RelayCommand
        (
            () => ShowFileDialog(new FileDialogEventArgs(FileDialogType.SaveFileDialog)
            {
                Filter = FileFilter,
                Callback = (r, e) => { if (r == true) SaveFile(e.FileName); }
            }),
            () => Editor.IsEditingFile
        );

        public ICommand OpenSlotCommand => new RelayCommand<int>
        (
            (i) => CloseFileRoutine(() => OpenFile(SaveSlots[i].Path))
        );

        public ICommand SaveSlotCommand => new RelayCommand<int>
        (
            (i) => SaveFile(SaveSlots[i].Path),
            (_) => Editor.IsEditingFile
        );

        public ICommand RefreshSlotsCommand => new RelayCommand
        (
            () => RefreshSaveSlots()
        );

        public ICommand ViewLogCommand => new RelayCommand
        (
            () => ShowLogWindow()
        );

        public ICommand ExitCommand => new RelayCommand
        (
            () => ExitAppWithConfirmation()
        );

        public ICommand EditCustomScripts => new RelayCommand
        (
            () =>
            {
                ShowCustomScriptsDialog();
                UpdateVisibleTabs();
            },
            () => Editor.IsEditingFile
        );

#if DEBUG
        private TabPageVM m_debugTab = null;
        private bool m_isDebugTabVisible = false;

        public bool IsDebugTabVisible
        {
            get { return m_isDebugTabVisible; }
            set { m_isDebugTabVisible = value; OnPropertyChanged(); }
        }

        public ICommand DebugShowHideDebugTab => new RelayCommand
        (
            () =>
            {
                if (m_debugTab == null)
                {
                    m_debugTab = new DebugTabVM() { TheWindow = this, Title = "Debug", Visibility = TabPageVisibility.Always };
                    m_debugTab.Init();
                }

                if (IsDebugTabVisible)
                {
                    Tabs.Add(m_debugTab);
                    m_debugTab.Load();
                }
                else
                {
                    m_debugTab.Unload();
                    Tabs.Remove(m_debugTab);
                }
                RefreshTabVisibility();
            }
        );

        public ICommand DebugSetDirtyCommand => new RelayCommand
        (
            () => { SetDirty(); Log.Debug("Dirty bit set."); },
            () => Editor.IsEditingFile
        );

        public ICommand DebugClearDirtyCommand => new RelayCommand
        (
            () => { ClearDirty(); Log.Debug("Dirty bit cleared."); },
            () => Editor.IsEditingFile
        );

        public ICommand DebugLoadCarColors => new RelayCommand
        (
            () =>
            {
                ShowFileDialog(FileDialogType.OpenFileDialog, (r, e) =>
                {
                    try
                    {
                        if (r != true) return;
                        GTA3.CarColors = CarColorsLoader.LoadColors(e.FileName);
                        SetTimedStatusText($"Loaded {GTA3.CarColors.Count()} car colors.");
                        UpdateVisibleTabs();
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        ShowException(ex, "An error occurred while loading car colors.");
                    }
                });
            }
        );

        public ICommand DebugLoadGxtTable => new RelayCommand
        (
            () =>
            {
                ShowFileDialog(FileDialogType.OpenFileDialog, (r, e) =>
                {
                    try
                    {
                        if (r != true) return;
                        GTA3.GxtTable = GxtLoader.Load(e.FileName);
                        SetTimedStatusText($"Loaded {GTA3.GxtTable.Count} GXT entries.");
                        UpdateVisibleTabs();
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        ShowException(ex, "An error occurred while loading GXT entries.");
                    }
                });
            }
        );

        public ICommand DebugLoadIdeObjects => new RelayCommand
        (
            () =>
            {
                ShowFileDialog(FileDialogType.OpenFileDialog, (r, e) =>
                {
                    try
                    {
                        if (r != true) return;
                        GTA3.IdeObjects = IdeLoader.LoadObjects(e.FileName);
                        SetTimedStatusText($"Loaded {GTA3.IdeObjects.Count()} IDE objects.");
                        UpdateVisibleTabs();
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                        ShowException(ex, "An error occurred while loading IDE objects.");
                    }
                });
            }
        );

        public ICommand DebugRaiseUnhandledException => new RelayCommand
        (
            () =>
            {
                Random r = new Random();
                int func = r.Next(0, 3);

                switch (func)
                {
                    // Any prospecting programers out there? Find the issue with each function!
                    case 0:
                    {
                        int[] foo = { 0, 1 };
                        foo[2] = 3;
                        break;
                    }
                    case 1:
                    {
                        using DataBuffer buf = new DataBuffer(10);
                        buf.Write(r.Next());
                        buf.Write(r.Next());
                        buf.Write(r.Next());
                        break;
                    }
                    case 2:
                    {
                        int.Parse("one");
                        break;
                    }
                }
            }
        );
#endif
    }
}
