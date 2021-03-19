using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Loaders;
using GTA3SaveEditor.Core.Util;
using GTA3SaveEditor.GUI.Types;
using GTASaveData.GTA3;
using WpfEssentials;
using WpfEssentials.Win32;
using System.Collections;
using System.Collections.Specialized;

namespace GTA3SaveEditor.GUI.Tabs
{
    // TODO: buildingswaps, invisibile objects, etc.


    public class ScriptsVM : TabPageVM
    {
        private Dictionary<int, string> m_symbols;
        private ObservableCollection<GlobalVarInfo> m_globals;
        private ObservableCollection<ThreadInfo> m_threads;
        private int m_selectedGlobalIndex;
        private int? m_selectedGlobalValue;
        private ThreadInfo m_selectedThread;
        private bool m_multipleGlobalsSelected;
        private bool m_multipleThreadsSelected;
        private int? m_localValue;
        private int? m_stackValue;
        private int m_localIndex;
        private int m_stackIndex;
        private bool m_suppressWriteLocal;
        private bool m_suppressWriteStack;
        private NumberFormat m_numFormat;

        public Dictionary<int, string> Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GlobalVarInfo> GlobalInfo
        {
            get { return m_globals; }
            set { m_globals = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ThreadInfo> ThreadInfo
        {
            get { return m_threads; }
            set { m_threads = value; OnPropertyChanged(); }
        }

        public int SelectedGlobalIndex
        {
            get { return m_selectedGlobalIndex; }
            set { m_selectedGlobalIndex = value; OnPropertyChanged(); }
        }

        public int? SelectedGlobalValue
        {
            get { return m_selectedGlobalValue; }
            set { m_selectedGlobalValue = value; OnPropertyChanged(); }
        }

        public ThreadInfo SelectedThread
        {
            get { return m_selectedThread; }
            set { m_selectedThread = value; OnPropertyChanged(); }
        }

        public bool MultipleGlobalsSelected
        {
            get { return m_multipleGlobalsSelected; }
            set { m_multipleGlobalsSelected = value; OnPropertyChanged(); }
        }

        public bool MultipleThreadsSelected
        {
            get { return m_multipleThreadsSelected; }
            set { m_multipleThreadsSelected = value; OnPropertyChanged(); }
        }

        public int LocalIndex
        {
            get { return m_localIndex; }
            set { m_localIndex = value; OnPropertyChanged(); }
        }

        public int StackIndex
        {
            get { return m_stackIndex; }
            set { m_stackIndex = value; OnPropertyChanged(); }
        }

        public int? LocalValue
        {
            get { return m_localValue; }
            set { m_localValue = value; OnPropertyChanged(); }
        }

        public int? StackValue
        {
            get { return m_stackValue; }
            set { m_stackValue = value; OnPropertyChanged(); }
        }

        public NumberFormat NumberFormat
        {
            get { return m_numFormat; }
            set { m_numFormat = value; OnPropertyChanged(); }
        }

        public ScriptsVM()
        {
            Symbols = new Dictionary<int, string>();
            GlobalInfo = new ObservableCollection<GlobalVarInfo>();
            ThreadInfo = new ObservableCollection<ThreadInfo>();
        }

        public override void Init()
        {
            base.Init();

            // TODO: allow user to choose
            LoadScmSymbols(IniLoader.LoadIni(App.LoadResource("CustomVariables.ini")));

            //GlobalInfo.CollectionChanged += GlobalInfo_CollectionChanged;
            //ThreadInfo.CollectionChanged += ThreadInfo_CollectionChanged;
        }

        public override void Shutdown()
        {
            base.Shutdown();

            //GlobalInfo.CollectionChanged -= GlobalInfo_CollectionChanged;
            //ThreadInfo.CollectionChanged -= ThreadInfo_CollectionChanged;
        }

        //private void ThreadInfo_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            foreach (var item in e.NewItems)
        //            {
        //                var t = item as ThreadInfo;
        //                SetDirty("Thread pool", t);
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            foreach (var item in e.NewItems)
        //            {
        //                var t = item as ThreadInfo;
        //                SetDirty("Thread pool", t);
        //            }
        //            break;
        //    }
        //}

        //private void GlobalInfo_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            foreach (var item in e.NewItems)
        //                MarkDirty("GlobalVars: Added", item as GlobalVarInfo);
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            foreach (var item in e.OldItems)
        //                MarkDirty("GlobalVars: Removed", item as GlobalVarInfo);
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            for (int i = 0; i < e.OldItems.Count; i++)
        //            {
        //                var o = e.OldItems[i] as GlobalVarInfo;
        //                var n = e.NewItems[i] as GlobalVarInfo;
        //                MarkDirty("GlobalVars: Changed", n, o);
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Move:
        //        case NotifyCollectionChangedAction.Reset:
        //            break;
        //    }
        //}

        public override void Load()
        {
            base.Load();

            SelectedThread = null;
            SelectedGlobalIndex = -1;
            LocalIndex = -1;
            StackIndex = -1;
        }

        public override void Update()
        {
            base.Update();
            ReadGlobals();
            ReadThreads();
        }

        public void ReadGlobals()
        {
            var globalValues = new List<int>(TheSave?.Scripts?.Globals);
            if (globalValues == null)
            {
                GlobalInfo.Clear();
                return;
            }

            SuppressDirty();
            GlobalInfo.Clear();
            for (int i = 0; i < globalValues.Count; i++)
            {
                if (!Symbols.TryGetValue(i, out string name))
                    name = "";

                GlobalInfo.Add(new GlobalVarInfo(name, i, globalValues[i]));
            }
            AllowDirty();
        }

        public void ReadThreads()
        {
            var actualThreads = TheSave?.Scripts?.Threads;
            if (actualThreads == null)
            {
                ThreadInfo = null;
                return;
            }

            var threadInfo = new ObservableCollection<ThreadInfo>();
            for (int i = 0; i < actualThreads.Count; i++)
            {
                threadInfo.Add(new ThreadInfo(i, actualThreads[i]));
            }

            ThreadInfo = threadInfo;
        }
        
        public void LoadScmSymbols(Dictionary<string, string> ini)
        {
            Symbols.Clear();
            foreach (var sym in ini)
            {
                if (!int.TryParse(sym.Key, out int index))
                {
                    Log.Warn($"Invalid index '{sym.Key}' for symbol '{sym.Value}'");
                }
                Symbols.Add(index, sym.Value);
            }
        }

        public void ReadLocalValue()
        {
            if (SelectedThread != null && LocalIndex >= 0)
            {
                m_suppressWriteLocal = true;
                LocalValue = SelectedThread.Thread.Locals[LocalIndex];
                m_suppressWriteLocal = false;
            }
            else
            {
                LocalValue = null;
            }
        }

        public void WriteLocalValue()
        {
            if (!m_suppressWriteLocal && SelectedThread != null && LocalIndex >= 0 && LocalValue.HasValue)
            {
                int oldIdx = LocalIndex;
                var t = SelectedThread.Thread;
                var i = SelectedThread.Index;
                t.Locals[LocalIndex] = LocalValue.Value;
                LocalIndex = oldIdx;
                MarkDirty($"{nameof(RunningScript)}[{i}].@{LocalIndex}", LocalValue.Value);
            }
        }

        public void ReadStackValue()
        {
            if (SelectedThread != null && StackIndex >= 0)
            {
                m_suppressWriteStack = true;
                StackValue = SelectedThread.Thread.Stack[StackIndex];
                m_suppressWriteStack = false;
            }
            else
            {
                StackValue = null;
            }
        }

        public void WriteStackValue()
        {
            if (!m_suppressWriteStack && SelectedThread != null && StackIndex >= 0 && StackValue.HasValue)
            {
                int oldIdx = StackIndex;
                var t = SelectedThread.Thread;
                var i = SelectedThread.Index;
                t.Stack[StackIndex] = StackValue.Value;
                StackIndex = oldIdx;
                MarkDirty($"{nameof(RunningScript)}[{i}].Stack[{StackIndex}]", StackValue.Value);
            }
        }

        public ICommand InsertThread => new RelayCommand
        (
            () =>
            {
                var actualThreads = TheSave.Scripts.Threads;
                int index = -1;
                
                if (SelectedThread != null)
                    index = actualThreads.IndexOf(SelectedThread.Thread);

                if (index == -1)
                    index = actualThreads.Count;

                var newThread = new RunningScript() { Name = Scripts.GenerateThreadName() };
                var newThreadInfo = new ThreadInfo(index, newThread);
                actualThreads.Insert(index, newThread);
                ThreadInfo.Insert(index, newThreadInfo);

                // TODO: scroll to new thread
                // TODO: dirty
            },
            () => !MultipleThreadsSelected
        );

        public ICommand DeleteThreads => new RelayCommand
        (
            () =>
            {
                var selected = ThreadInfo.Where(t => t.IsSelected).ToList();
                if (selected.Count == 1)
                    Debug.Assert(SelectedThread.Equals(selected.First()));

                var actualThreads = TheSave.Scripts.Threads;
                foreach (var info in selected)
                {
                    actualThreads.Remove(info.Thread);
                    ThreadInfo.Remove(info);
                }
            },
            () => SelectedThread != null || MultipleThreadsSelected
        );

        public ICommand InsertGlobal => new RelayCommand
        (
            () =>
            {
                int index = SelectedGlobalIndex;

                var actualGlobals = TheSave.Scripts.Globals.ToList();
                if (index < 0)
                    index = actualGlobals.Count;

                // Insert into local copy
                actualGlobals.Insert(index, 0);
                MarkDirty($"{nameof(TheSave.Scripts.Globals)}[{index}]", 0);

                // Update the global variables / script space pool
                TheSave.Scripts.SetScriptSpace(actualGlobals);
                ReadGlobals();

                // TODO: scroll to new variable
            },
            () => !MultipleGlobalsSelected
        );

        public ICommand InsertGlobalMultiple => new RelayCommand
        (
            () =>
            {
                TheWindow.ShowInputDialog("Enter amount:", out string amountString);
                int.TryParse(amountString, out int amount);

                int index = SelectedGlobalIndex;

                var actualGlobals = TheSave.Scripts.Globals.ToList();
                if (index < 0)
                    index = actualGlobals.Count;

                for (int i = 0; i < amount; i++)    // TODO: limit?
                {
                    // Insert into local copy
                    actualGlobals.Insert(index + i, 0);
                    MarkDirty($"{nameof(TheSave.Scripts.Globals)}[{index + i}]", 0);
                }

                // Update the global variables / script space pool
                TheSave.Scripts.SetScriptSpace(actualGlobals);
                ReadGlobals();

                // TODO: scroll to new variables
            }
        );

        public ICommand DeleteGlobals => new RelayCommand<object>
        (
            (x) =>
            {
                var selected = (x as IList)?.Cast<GlobalVarInfo>().OrderByDescending(g => g.Index).ToList();
                if (selected == null)
                    return;

                if (selected.Count == 1)
                    Debug.Assert(SelectedGlobalValue.Equals(selected.First().Value));

                var globals = TheSave.Scripts.Globals.ToList();
                foreach (var g in selected)
                {
                    // remove from local copy
                    globals.RemoveAt(g.Index);
                    MarkDirty($"Remove {nameof(TheSave.Scripts.Globals)}[{g.Index}]");
                }

                // Update the global variables / script space pool
                TheSave.Scripts.SetScriptSpace(globals);
                ReadGlobals();

                // TODO: dirty
            },
            (_) => SelectedGlobalValue != null || MultipleGlobalsSelected
        );
    }



    // TODO: move this elsewhere

    public class ListViewItemInfo : ObservableObject
    {
        private bool m_isSelected;
        public bool IsSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; OnPropertyChanged(); }
        }
    }

    public class GlobalVarInfo : ListViewItemInfo
    {
        private string m_name;
        private int m_index;
        private int? m_value;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged(); }
        }

        public int Index
        {
            get { return m_index; }
            private set { m_index = value; OnPropertyChanged(); }
        }

        public int? Value
        {
            get { return m_value; }
            set { m_value = value; OnPropertyChanged(); }
        }

        public GlobalVarInfo(string name, int index, int value)
        {
            Name = name;
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return $"GlobalVar[{Index}]: {Name}, {Value}";
        }
    }

    public class ThreadInfo : ListViewItemInfo
    {
        private RunningScript m_thread;
        private int m_index;

        public RunningScript Thread
        {
            get { return m_thread; }
            set { m_thread = value; OnPropertyChanged(); }
        }

        public int Index
        {
            get { return m_index; }
            private set { m_index = value; OnPropertyChanged(); }
        }

        public ThreadInfo(int index, RunningScript thread)
        {
            Thread = thread;
            Index = index;
        }

        public override string ToString()
        {
            return $"Thread[{Index}]: {Thread.Name}, {Thread.IP}";
        }
    }
}

