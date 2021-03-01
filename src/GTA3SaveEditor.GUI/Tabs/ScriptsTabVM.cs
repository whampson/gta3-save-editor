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

namespace GTA3SaveEditor.GUI.Tabs
{
    // TODO: buildingswaps, invisibile objects, etc.


    public class ScriptsTabVM : TabPageVM
    {
        private Dictionary<int, string> m_symbols;
        private ObservableCollection<VarInfo> m_globals;
        private ObservableCollection<ThreadInfo> m_threads;
        private VarInfo m_selectedGlobal;
        private ThreadInfo m_selectedThread;
        private bool m_multipleGlobalsSelected;
        private bool m_multipleThreadsSelected;
        private int? m_localValue;
        private int? m_stackValue;
        private int m_localIndex;
        private int m_stackIndex;
        private bool m_suppressReadGlobal;
        private bool m_suppressWriteGlobal;
        private bool m_suppressWriteLocal;
        private bool m_suppressWriteStack;
        private NumberFormat m_numFormat;

        public Dictionary<int, string> Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; OnPropertyChanged(); }
        }

        public ObservableCollection<VarInfo> GlobalInfo
        {
            get { return m_globals; }
            set { m_globals = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ThreadInfo> ThreadInfo
        {
            get { return m_threads; }
            set { m_threads = value; OnPropertyChanged(); }
        }

        public VarInfo SelectedGlobal
        {
            get { return m_selectedGlobal; }
            set { m_selectedGlobal = value; OnPropertyChanged(); }
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

        public ScriptsTabVM()
        {
            Symbols = new Dictionary<int, string>();
            GlobalInfo = new ObservableCollection<VarInfo>();
            ThreadInfo = new ObservableCollection<ThreadInfo>();
        }

        public override void Init()
        {
            base.Init();

            // TODO: allow user to choose
            LoadSymbols(IniLoader.LoadIni(App.LoadResource("CustomVariables.ini")));
        }

        public override void Load()
        {
            base.Load();

            SelectedGlobal = null;
            SelectedThread = null;
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
                GlobalInfo = null;
                return;
            }

            var globalInfo = new ObservableCollection<VarInfo>();
            for (int i = 0; i < globalValues.Count; i++)
            {
                if (!Symbols.TryGetValue(i, out string name))
                    name = "";

                globalInfo.Add(new VarInfo(name, i, globalValues[i]));
            }

            GlobalInfo = globalInfo;
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
            foreach (var t in actualThreads)
            {
                threadInfo.Add(new ThreadInfo(t));
            }

            ThreadInfo = threadInfo;
        }
        
        public void LoadSymbols(Dictionary<string, string> ini)
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

        public void ReadSelectedGlobalValue()
        {
            if (m_suppressReadGlobal || SelectedGlobal == null) return;

            int index = SelectedGlobal.Index;
            if (index >= 0)
            {
                m_suppressWriteGlobal = true;
                SelectedGlobal.Value = TheSave.Scripts.GetGlobal(index);
                m_suppressWriteGlobal = false;
            }
            else
            {
                SelectedGlobal.Value = null;
            }
        }

        public void WriteGlobalValue()
        {
            if (m_suppressWriteGlobal || SelectedGlobal == null) return;

            // TODO: this gets called too often
            // TODO: bug also allows null value until item selected again

            int index = SelectedGlobal.Index;
            if (index >= 0 && SelectedGlobal.Value.HasValue)
            {
                m_suppressWriteGlobal = true;
                int value = SelectedGlobal.Value.Value;
                GlobalInfo[index].Value = value;
                TheSave.Scripts.SetGlobal(index, value);
                m_suppressWriteGlobal = false;
                TheWindow.SetDirty();       // TODO: subscribe to Dirty event in main window
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
                SelectedThread.Thread.Locals[LocalIndex] = LocalValue.Value;
                LocalIndex = oldIdx;
                TheWindow.SetDirty();
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
                SelectedThread.Thread.Stack[StackIndex] = StackValue.Value;
                StackIndex = oldIdx;
                TheWindow.SetDirty();
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
                actualThreads.Insert(index, newThread);

                var newThreadInfo = new ThreadInfo(newThread);
                ThreadInfo.Insert(index, newThreadInfo);
                // TODO: scroll to new thread
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
                int index = -1;
                if (SelectedGlobal != null)
                    index = SelectedGlobal.Index;

                var actualGlobals = TheSave.Scripts.Globals.ToList();
                if (index < 0)
                    index = actualGlobals.Count;

                actualGlobals.Insert(index, 0);

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

                int index = -1;
                if (SelectedGlobal != null)
                    index = SelectedGlobal.Index;

                var actualGlobals = TheSave.Scripts.Globals.ToList();
                if (index < 0)
                    index = actualGlobals.Count;

                for (int i = 0; i < amount; i++)    // TODO: limit?
                    actualGlobals.Insert(index + i, 0);

                TheSave.Scripts.SetScriptSpace(actualGlobals);
                ReadGlobals();
                // TODO: scroll to new variables
            }
        );

        public ICommand DeleteGlobals => new RelayCommand<object>
        (
            (x) =>
            {
                var selected = (x as IList)?.Cast<VarInfo>().OrderByDescending(g => g.Index).ToList();
                if (selected == null)
                    return;

                if (selected.Count == 1)
                    Debug.Assert(SelectedGlobal.Equals(selected.First()));

                var globals = TheSave.Scripts.Globals.ToList();
                foreach (var g in selected)
                {
                    globals.RemoveAt(g.Index);
                }

                TheSave.Scripts.SetScriptSpace(globals);
                ReadGlobals();
            },
            (_) => SelectedGlobal != null || MultipleGlobalsSelected
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

    public class VarInfo : ListViewItemInfo
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
            set { m_index = value; OnPropertyChanged(); }
        }

        public int? Value
        {
            get { return m_value; }
            set { m_value = value; OnPropertyChanged(); }
        }

        public VarInfo()
        { }

        public VarInfo(string name, int index, int value)
        {
            Name = name;
            Index = index;
            Value = value;
        }
    }

    public class ThreadInfo : ListViewItemInfo
    {
        private RunningScript m_thread;

        public RunningScript Thread
        {
            get { return m_thread; }
            set { m_thread = value; OnPropertyChanged(); }
        }

        public ThreadInfo()
        { }

        public ThreadInfo(RunningScript thread)
        {
            Thread = thread;
        }
    }
}

