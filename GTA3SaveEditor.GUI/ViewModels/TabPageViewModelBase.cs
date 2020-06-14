using GTA3SaveEditor.GUI.Events;
using WpfEssentials;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public abstract class TabPageViewModelBase : ObservableObject
    {
        public string Title { get; }
        public TabPageVisibility Visibility { get; }
        public MainViewModel MainViewModel { get; }

        private bool m_isVisible;
        public bool IsVisible
        {
            get { return m_isVisible; }
            set { m_isVisible = value; OnPropertyChanged(); }
        }

        public TabPageViewModelBase(string title, TabPageVisibility visibility, MainViewModel mainViewModel)
        {
            Title = title;
            Visibility = visibility;
            IsVisible = visibility == TabPageVisibility.Always;
            MainViewModel = mainViewModel;
            MainViewModel.TabRefresh += MainViewModel_TabRefresh;
        }

        private void MainViewModel_TabRefresh(object sender, TabRefreshEventArgs e)
        {
            if (Visibility == TabPageVisibility.Always)
            {
                IsVisible = true;
                return;
            }

            switch (e.Trigger)
            {
                case TabRefreshTrigger.WindowLoaded:
                case TabRefreshTrigger.FileClosed:
                    IsVisible = (Visibility == TabPageVisibility.WhenFileIsClosed);
                    break;
                case TabRefreshTrigger.FileOpened:
                    IsVisible = (Visibility == TabPageVisibility.WhenFileIsOpen);
                    break;
            }
        }
    }

    /// <summary>
    /// Tab page visibility types.
    /// </summary>
    public enum TabPageVisibility
    {
        /// <summary>
        /// Indicates that a tab page should always be visible.
        /// </summary>
        Always,

        /// <summary>
        /// Indicates that a tab page should be visible only when editing a file.
        /// </summary>
        WhenFileIsOpen,

        /// <summary>
        /// Indicates that a tab page should be visibie only when not editing a file.
        /// </summary>
        WhenFileIsClosed
    }
}
