using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Newtonsoft.Json;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI
{
    public class DummyVM :   TabPageVM
    { }

    public class MainVM : BaseWindowVM
    {
        private const int NumSaveSlots = 8;
        private const string FileFilter = "GTA3 Save Files (*.b)|*.b|All Files|*.*";

        private const string TabNameWelcome = "Welcome";
        private const string TabNameGeneral = "General";
        private const string TabNameGarages = "Garages";
        private const string TabNamePeds = "Gangs & Peds";
        private const string TabNamePickups = "Pickups";
        private const string TabNameScripts = "Scripts";

        private ObservableCollection<SaveSlot> m_saveSlots;
        private ObservableCollection<TabPageVM> m_tabs;
        private int m_selectedTabIndex;
        private bool m_isReverting;
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

        public MainVM()
        {
            var slots = Enumerable.Range(0, NumSaveSlots).Select(slot => new SaveSlot());
            SaveSlots = new ObservableCollection<SaveSlot>(slots);
            Tabs = ConstructTabs();
        }

        private ObservableCollection<TabPageVM> ConstructTabs()
        {
            return new ObservableCollection<TabPageVM>()
            {
                new WelcomeVM()  { TheWindow = this, Title = TabNameWelcome, Visibility = TabPageVisibility.WhenNotEditingFile },
                new GeneralVM()  { TheWindow = this, Title = TabNameGeneral, Visibility = TabPageVisibility.WhenEditingFile },      // Simple Variables
                new GaragesVM()  { TheWindow = this, Title = TabNameGarages, Visibility = TabPageVisibility.WhenEditingFile },      // Save Garages
                new PedsVM()    { TheWindow = this, Title = TabNamePeds,   Visibility = TabPageVisibility.WhenEditingFile },      // Gangs
                new PickupsVM()  { TheWindow = this, Title = TabNamePickups, Visibility = TabPageVisibility.WhenEditingFile },      // All pickups
                new ScriptsVM()  { TheWindow = this, Title = TabNameScripts, Visibility = TabPageVisibility.WhenEditingFile },      // GlobalVars and Threads
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
            UpdateTabVisibility();
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

        public T GetTab<T>() where T : TabPageVM
        {
            return (T) Tabs.Where(t => t is T).FirstOrDefault();
        }

        private void InitAllTabs()
        {
            foreach (var tab in Tabs)
            {
                tab.Init();
                tab.Dirty += MarkDirty;
            }
        }

        private void ShutdownAllTabs()
        {
            foreach (var tab in Tabs)
            {
                tab.Dirty -= MarkDirty;
                tab.Shutdown();
            }
        }

        private void UpdateTabs()
        {
            foreach (var tab in Tabs)
            {
                if (tab.IsVisible)
                {
                    tab.Update();
                }
            }
        }

        private void UpdateSelectedTab()
        {
            if (SelectedTabIndex != -1)
            {
                var tab = Tabs[SelectedTabIndex];
                tab.Update();
            }
        }

        private void ReloadTabs()
        {
            foreach (var tab in Tabs)
            {
                if (tab.IsVisible)
                {
                    tab.Unload();
                    tab.Load();
                }
            }
        }

        private void UpdateTabVisibility()
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
                    SaveEditor.TryLoadFile(slot.Path, out GTA3Save save);
                    slot.Name = save?.Title ?? $"(invalid save file)";
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

        public void MarkDirty(string name, object value = null, object oldValue = null)
        {
            string msg = $"{name}";
            if (value != null) msg += $" = {value}";
            if (oldValue != null) msg += $" (was {oldValue})";

            Log.Info(msg);

            if (!IsDirty)
            {
                IsDirty = true;
                UpdateTitle();
                Log.Debug("Dirty bit set.");
            }
        }

        private void ClearDirty()
        {
            IsDirty = false;
            UpdateTitle();
            Log.Debug("Dirty bit cleared.");
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

        public void OpenFileRequest_Handler(object sender, FileIOEventArgs e)
        {
            if (IsDirty)
            {
                CloseFileRoutine(() => OpenFileRoutine(e.Path), suppressDialogs: e.SuppressDialogs);
                return;
            }

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
            // TODO: dim the layout or pop a 'wait' dialog or something
            RevertFileRoutine();
            ShowInfo("File refreshed.");
            SetTimedStatusText("File reverted.");

        }

        private void FileOpening_Handler(object sender, string e)
        {
            SetTimedStatusText("Opening file...");
        }

        private void FileOpened_Handler(object sender, EventArgs e)
        {
            SuppressExternalChangesCheck = false;
            
            RegisterDirtyHandlers(TheSave);

            if (m_isReverting)
            {
                
                ReloadTabs();
                UpdateSelectedTab();
            }
            else
            {
                UpdateTitle();
                UpdateTabVisibility();
                SelectFirstVisibleTab();
            }

            OnPropertyChanged(nameof(TheSave));
            SetTimedStatusText("File opened.");

            string lastMissionKey = TheSave.Stats.LastMissionPassedName;
            string lastMission = GTA3.GetGxtString(lastMissionKey);
            Log.Info($"=============== FILE INFO ===============");
            Log.Info($"    Platform: {TheSave.GetFileType()}");
            Log.Info($"  Time Stamp: {TheSave.TimeStamp}");
            Log.Info($"        Name: {TheSave.GetTitle()}");
            Log.Info($"Last Mission: {lastMission}");
            Log.Info($"    Progress: {((float) TheSave.Stats.ProgressMade / TheSave.Stats.TotalProgressInGame):P2}");
            Log.Info($"   MAIN Size: {TheSave.Script.MainScriptSize}");
            Log.Info($"Num. Globals: {TheSave.Script.GlobalVariables.Count()}");
            Log.Info($"Num. Threads: {TheSave.Script.RunningScripts.Count}");
            Log.Info($"Num. Objects: {TheSave.Objects.Objects.Count}");
            Log.Info($"=========================================");
        }

        private void FileClosing_Handler(object sender, EventArgs e)
        {
            SetTimedStatusText("Closing file...");
            UnregisterDirtyHandlers(TheSave);

            ClearDirty();
        }

        private void FileClosed_Handler(object sender, EventArgs e)
        {
            SuppressExternalChangesCheck = false;

            if (!m_isReverting)
            {
                UpdateTitle();
                UpdateTabVisibility();
                ReloadTabs();
                SelectFirstVisibleTab();
            }

            OnPropertyChanged(nameof(TheSave));
            SetTimedStatusText("File closed.", expiredStatus: null);
        }

        private void FileSaving_Handler(object sender, string e)
        {
            SetTimedStatusText("Saving file...");

            if (IsDirty)
            {
                #pragma warning disable CS0618
                TheSave.Garages.BankVansCollected = 0x41454843;
                TheSave.Garages.PoliceCarsCollected = 0x00524554;
                #pragma warning restore CS0618
            }
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
                goto Done;
            }

            // prompt
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

        Done:
            onFileClosed?.Invoke();
            return true;
        }

        private void RevertFileRoutine()
        {
            // prompt
            if (IsDirty)
            {
                PromptYesNo("Are you sure you want to revert this file? You will lose all unsaved changes.", "Confirm Revert",
                    yesAction: () =>
                    {
                        ClearDirty();
                        RevertFileRoutine();
                    });
                return;
            }

            m_isReverting = true;
            CloseFileRoutine(() => OpenFileRoutine(Editor.ActiveFilePath), suppressDialogs: true);
            m_isReverting = false;
        }

        private void RegisterDirtyHandlers(INotifyPropertyChanged o)
        {
            if (o == null) return;

            foreach (var p in o.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0))
            {
                object v = p.GetValue(o, null);

                // Exclude fully-replaceable lists
                if (v == TheSave.Script.ScriptSpace)
                {
                    continue;
                }

                if (p.PropertyType.GetInterface(nameof(INotifyItemStateChanged)) != null)
                {
                    (v as INotifyItemStateChanged).ItemStateChanged += TheSave_CollectionElementChanged;
                    Log.Debug($"Registered ItemStateChanged handler on '{p.Name}' ({p.PropertyType})");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) != null)
                {
                    (v as INotifyCollectionChanged).CollectionChanged += TheSave_CollectionChanged;
                    Log.Debug($"Registered CollectionChanged handler on '{p.Name}' ({p.PropertyType})");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
                {
                    RegisterDirtyHandlers(v as INotifyPropertyChanged);
                }
            }
            o.PropertyChanged += TheSave_PropertyChanged;
        }

        private void UnregisterDirtyHandlers(INotifyPropertyChanged o)
        {
            if (o == null) return;

            foreach (var p in o.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0))
            {
                object v = p.GetValue(o, null);

                // Exclude fully-replaceable lists
                if (v == TheSave.Script.ScriptSpace)
                {
                    continue;
                }

                if (p.PropertyType.GetInterface(nameof(INotifyItemStateChanged)) != null)
                {
                    (v as INotifyItemStateChanged).ItemStateChanged -= TheSave_CollectionElementChanged;
                    Log.Debug($"Unregistered ItemStateChanged handler on '{p.Name}' ({p.PropertyType})");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyCollectionChanged)) != null)
                {
                    (v as INotifyCollectionChanged).CollectionChanged -= TheSave_CollectionChanged;
                    Log.Debug($"Unregistered CollectionChanged handler on '{p.Name}' ({p.PropertyType})");
                }
                if (p.PropertyType.GetInterface(nameof(INotifyPropertyChanged)) != null)
                {
                    UnregisterDirtyHandlers(v as INotifyPropertyChanged);
                }
            }
            o.PropertyChanged -= TheSave_PropertyChanged;
        }

        private void TheSave_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var type = sender.GetType();
            var prop = type.GetProperty(e.PropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop.GetIndexParameters().Length == 0)
            {
                // Only deal with non-indexer properties
                var data = prop.GetValue(sender);

                // Handle special cases
                if (type.Name == nameof(SimpleVariables) &&
                    e.PropertyName == nameof(TheSave.SimpleVars.LastMissionPassedName) &&
                    TheSave.IsTitleGxtKey())
                {
                    data = $"({TheSave.GetTitleRaw()}, {TheSave.GetTitle()})";
                }

                MarkDirty($"{type.Name}.{e.PropertyName}", data);
            }
        }

        private void TheSave_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var type = sender.GetType().GetGenericArguments()[0];
            var name = type.Name;

            //if (sender == TheSave.Scripts.ScriptSpace)
            //{
            //    name = $"{nameof(Scripts)}.ScriptSpace";
            //}
            //else if (sender == Stats.PedsKilledOfThisType)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.PedsKilledOfThisType)}";
            //}
            //else if (sender == Stats.BestBanditLapTimes)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.BestBanditLapTimes)}";
            //}
            //else if (sender == Stats.BestBanditPositions)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.BestBanditPositions)}";
            //}
            //else if (sender == Stats.BestStreetRacePositions)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.BestStreetRacePositions)}";
            //}
            //else if (sender == Stats.FastestStreetRaceLapTimes)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.FastestStreetRaceLapTimes)}";
            //}
            //else if (sender == Stats.FastestStreetRaceTimes)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.FastestStreetRaceTimes)}";
            //}
            //else if (sender == Stats.FastestDirtBikeLapTimes)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.FastestDirtBikeLapTimes)}";
            //}
            //else if (sender == Stats.FastestDirtBikeTimes)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.FastestDirtBikeTimes)}";
            //}
            //else if (sender == Stats.FavoriteRadioStationList)
            //{
            //    name = $"{nameof(Stats)}.{nameof(Stats.FavoriteRadioStationList)}";
            //}

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var data = e.NewItems[i];
                        if (data is SaveDataObject o) data = o.ToJsonString(Formatting.None);
                        MarkDirty($"{name}[{e.NewStartingIndex + i}]", data);
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var data = e.OldItems[i];
                        if (data is SaveDataObject o) data = o.ToJsonString(Formatting.None);
                        MarkDirty($"Removed {name}[{e.OldStartingIndex + i}]", oldValue: data);
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var data = e.NewItems[i];
                        if (data is SaveDataObject o) data = o.ToJsonString(Formatting.None);
                        MarkDirty($"{name}[{e.NewStartingIndex + i}]", data);
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        MarkDirty($"{name}[{e.OldStartingIndex + i}] => {name}[{e.NewStartingIndex + i}]");
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    MarkDirty($"Cleared {name}");
                    break;
                }
            }
        }

        private void TheSave_CollectionElementChanged(object sender, ItemStateChangedEventArgs e)
        {
            if (!(sender is IList list))
            {
                return;
            }

            var item = list[e.ItemIndex];
            var type = item.GetType();
            var prop = type.GetProperty(e.PropertyName, BindingFlags.Public | BindingFlags.Instance);
            var data = prop.GetValue(item);
            if (data is SaveDataObject o) data = o.ToJsonString(Formatting.None);

            MarkDirty($"{type.Name}[{e.ItemIndex}].{e.PropertyName}", data);
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
                UpdateTabs();
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
                    m_debugTab = new DebugVM() { TheWindow = this, Title = "Debug", Visibility = TabPageVisibility.Always };
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
                UpdateTabVisibility();
            }
        );

        public ICommand DebugSetDirtyCommand => new RelayCommand
        (
            () => { MarkDirty(nameof(DebugSetDirtyCommand), true); },
            () => Editor.IsEditingFile
        );

        public ICommand DebugClearDirtyCommand => new RelayCommand
        (
            () => { ClearDirty(); },
            () => Editor.IsEditingFile
        );

        public ICommand DebugCommandConvertScriptsToV0 => new RelayCommand
        (
            () => TheSave.SetScriptVersion(ScmVersion.SCMv0),
            () => Editor.IsEditingFile
        );

        public ICommand DebugCommandConvertScriptsToV1 => new RelayCommand
        (
            () => TheSave.SetScriptVersion(ScmVersion.SCMv1),
            () => Editor.IsEditingFile
        );

        public ICommand DebugCommandConvertScriptsToV2 => new RelayCommand
        (
            () => TheSave.SetScriptVersion(ScmVersion.SCMv2),
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
                        UpdateTabs();
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
                        UpdateTabs();
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
                        UpdateTabs();
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
