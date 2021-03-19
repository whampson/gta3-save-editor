using GTA3SaveEditor.Core;
using GTASaveData.GTA3;
using WHampson.ToolUI;

namespace GTA3SaveEditor.GUI
{
    public abstract class TabPageVM : ViewModelBase
    {
        private MainWindowVM m_mainWindow;
        private TabPageVisibility m_visibility;
        private bool m_isVisible;
        private string m_title;
        private bool m_suppressDirty;

        public SaveEditor Editor => SaveEditor.Instance;
        public Settings Settings => SaveEditor.Settings;
        public SaveFileGTA3 TheSave => SaveEditor.Instance.ActiveFile;

        public delegate void DirtyChangedHandler(string propertyName, object value, object oldValue);
        public event DirtyChangedHandler Dirty;

        public MainWindowVM TheWindow
        {
            get { return m_mainWindow; }
            set { m_mainWindow = value; OnPropertyChanged(); }
        }

        public TabPageVisibility Visibility
        {
            get { return m_visibility; }
            set { m_visibility = value; OnPropertyChanged(); }
        }

        public bool IsVisible
        {
            get { return m_isVisible; }
            set { SetVisible(value); OnPropertyChanged(); }
        }

        public string Title
        {
            get { return m_title; }
            set { m_title = value; OnPropertyChanged(); }
        }

        public bool IsDirtySuppressed => m_suppressDirty;

        private void SetVisible(bool isVisible)
        {
            bool wasVisible = m_isVisible;
            if (wasVisible && !isVisible)
            {
                Unload();
            }
            if (isVisible && !wasVisible)
            {
                Load();
            }

            m_isVisible = isVisible;
        }

        public void MarkDirty(string message, object value = null, object oldValue = null)
        {
            if (!m_suppressDirty)
            {
                Dirty?.Invoke(message, value, oldValue);
            }
        }

        protected void AllowDirty() => m_suppressDirty = false;
        protected void SuppressDirty() => m_suppressDirty = true;
    }

    public enum TabPageVisibility
    {
        Always,
        WhenEditingFile,
        WhenNotEditingFile
    }
}
