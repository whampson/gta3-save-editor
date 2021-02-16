using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Loaders;
using GTA3SaveEditor.Core.Util;
using GTA3SaveEditor.GUI.Types;
using GTASaveData.GTA3;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Tabs
{
    public class ScriptsTabVM : TabPageVM
    {
        // TODO: buildingswaps, invisibile objects, etc.

        private Dictionary<int, string> m_symbols;
        private ObservableCollection<int> m_globals;
        private RunningScript m_thread;
        private int m_globalIndex;
        private int m_localIndex;
        private int m_stackIndex;
        private int m_threadIndex;
        private int? m_globalValue;
        private int? m_localValue;
        private int? m_stackValue;
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

        public ObservableCollection<int> Globals
        {
            get { return m_globals; }
            set { m_globals = value; OnPropertyChanged(); }
        }

        public RunningScript Thread
        {
            get { return m_thread; }
            set { m_thread = value; OnPropertyChanged(); }
        }

        public int GlobalIndex
        {
            get { return m_globalIndex; }
            set { m_globalIndex = value; OnPropertyChanged(); }
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

        public int ThreadIndex
        {
            get { return m_threadIndex; }
            set { m_threadIndex = value; OnPropertyChanged(); }
        }

        public int? GlobalValue
        {
            get { return m_globalValue; }
            set { m_globalValue = value; OnPropertyChanged(); }
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
            Globals = new ObservableCollection<int>();
            Symbols = new Dictionary<int, string>();
        }

        public override void Init()
        {
            base.Init();

            LoadSymbols(IniLoader.LoadIni(App.LoadResource("CustomVariables.ini")));
        }

        public override void Load()
        {
            base.Load();

            GlobalIndex = -1;
            LocalIndex = -1;
            StackIndex = -1;
            m_threadIndex = -1;
        }

        public override void Update()
        {
            base.Update();

            UpdateGlobals();
        }

        public void UpdateGlobals()
        {
            IEnumerable<int> g = TheSave?.Scripts?.Globals;
            Globals = (g == null) ? null : new ObservableCollection<int>(g);
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

        public void ReadGlobalValue()
        {
            if (m_suppressReadGlobal) return;

            if (Globals != null && GlobalIndex >= 0)
            {
                m_suppressWriteGlobal = true;
                GlobalValue = Globals[GlobalIndex];
                m_suppressWriteGlobal = false;
            }
            else
            {
                GlobalValue = null;
            }
        }

        public void WriteGlobalValue()
        {
            if (m_suppressWriteGlobal) return;

            if (Globals != null && GlobalIndex >= 0 && GlobalValue.HasValue)
            {
                int oldIdx = GlobalIndex;
                m_suppressReadGlobal = true;
                int value = GlobalValue.Value;
                Globals[oldIdx] = value;
                TheSave.Scripts.SetGlobal(oldIdx, value);

                GlobalIndex = oldIdx;
                m_suppressReadGlobal = false;
                TheWindow.SetDirty();
            }
        }

        public void ReadLocalValue()
        {
            if (Thread != null && LocalIndex >= 0)
            {
                m_suppressWriteLocal = true;
                LocalValue = Thread.Locals[LocalIndex];
                m_suppressWriteLocal = false;
            }
            else
            {
                LocalValue = null;
            }
        }

        public void WriteLocalValue()
        {
            if (!m_suppressWriteLocal && Thread != null && LocalIndex >= 0 && LocalValue.HasValue)
            {
                int oldIdx = LocalIndex;
                Thread.Locals[LocalIndex] = LocalValue.Value;
                LocalIndex = oldIdx;
                TheWindow.SetDirty();
            }
        }

        public void ReadStackValue()
        {
            if (Thread != null && StackIndex >= 0)
            {
                m_suppressWriteStack = true;
                StackValue = Thread.Stack[StackIndex];
                m_suppressWriteStack = false;
            }
            else
            {
                StackValue = null;
            }
        }

        public void WriteStackValue()
        {
            if (!m_suppressWriteStack && Thread != null && StackIndex >= 0 && StackValue.HasValue)
            {
                int oldIdx = StackIndex;
                Thread.Stack[StackIndex] = StackValue.Value;
                StackIndex = oldIdx;
                TheWindow.SetDirty();
            }
        }

        public ICommand InsertThread => new RelayCommand
        (
            () =>
            {
                int idx = ThreadIndex;
                if (idx == -1) idx = TheSave.Scripts.Threads.Count;

                var t = new RunningScript() { Name = Scripts.GenerateThreadName() };
                TheSave.Scripts.Threads.Insert(idx, t);
                Thread = t;
            }
        );

        public ICommand DeleteSelectedThreads => new RelayCommand
        (
            () =>
            {
                // TODO: multiple
                int idx = ThreadIndex;
                Thread = null;
                TheSave.Scripts.Threads.RemoveAt(idx);
            },
            () => ThreadIndex != -1
        );
    }
}
