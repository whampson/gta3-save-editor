using System;
using GTASaveData.GTA3;
using WpfEssentials;

namespace GTA3SaveEditor.Core.Game
{
    public class CustomScript : ObservableObject
    {
        private  GTA3Script m_code;
        private RunningScript m_thread;
        private bool m_enabled;

        public GTA3Script Code
        {
            get { return m_code; }
            set { m_code = value; OnPropertyChanged(); }
        }

        public RunningScript Thread
        {
            get { return m_thread; }
            set { m_thread = value; OnPropertyChanged(); }
        }
        
        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; OnPropertyChanged(); }
        }

        public CustomScript(GTA3Script code)
            : this(code, null, 0, false)
        { }

        public CustomScript(GTA3Script code, RunningScript thread, int baseAddr, bool enabled)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Thread = thread ?? new RunningScript() { Name = Code.Name, IP = baseAddr };
            Enabled = enabled;
        }
    }
}
