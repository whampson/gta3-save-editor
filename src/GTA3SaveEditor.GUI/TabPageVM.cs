using GTA3SaveEditor.Core;
using GTASaveData.GTA3;
using WHampson.ToolUI;

namespace GTA3SaveEditor.GUI
{
    public abstract class TabPageVM : UserControlViewModelBase
    {
        private MainWindowVM m_mainWindow;
        private TabPageVisibility m_visibility;
        private bool m_isVisible;
        private string m_title;

        public SaveEditor Editor => SaveEditor.Instance;
        public Settings Settings => SaveEditor.Settings;
        public SaveFileGTA3 TheSave => SaveEditor.Instance.ActiveFile;

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
            private set { SetVisible(value); OnPropertyChanged(); }
        }

        public string Title
        {
            get { return m_title; }
            set { m_title = value; OnPropertyChanged(); }
        }

        public void UpdateVisibility()
        {
            IsVisible =
                (Visibility == TabPageVisibility.Always) ||
                (Visibility == TabPageVisibility.WhenEditingFile && SaveEditor.Instance.IsEditingFile) ||
                (Visibility == TabPageVisibility.WhenNotEditingFile && !SaveEditor.Instance.IsEditingFile);
        }

        private void SetVisible(bool isVisible)
        {
            bool wasVisible = m_isVisible;
            m_isVisible = isVisible;

            if (wasVisible && !m_isVisible)
            {
                Unload();
            }
            if (m_isVisible && !wasVisible)
            {
                Load();
                Update();
            }
        }
    }

    public enum TabPageVisibility
    {
        Always,
        WhenEditingFile,
        WhenNotEditingFile
    }
}
