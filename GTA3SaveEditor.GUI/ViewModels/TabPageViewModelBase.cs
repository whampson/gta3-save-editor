using GTA3SaveEditor.GUI.Events;
using WpfEssentials;

namespace GTA3SaveEditor.GUI.ViewModels
{
    /// <summary>
    /// The view model base for tab pages.
    /// </summary>
    public abstract class TabPageViewModelBase : ObservableObject
    {
        private bool m_isVisible;

        /// <summary>
        /// Gets or sets whether the tab page is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return m_isVisible; }
            set { m_isVisible = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets the tab page visibility setting. This dicates whether the page will
        /// be visible or hidden when a <see cref="MainViewModel.TabRefresh"/> event occurs.
        /// </summary>
        public TabPageVisibility Visibility { get; }

        /// <summary>
        /// Gets the tab name.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the main window view model for accessing global functions.
        /// </summary>
        public MainViewModel MainViewModel { get; }

        public TabPageViewModelBase(string title, TabPageVisibility visibility, MainViewModel mainViewModel)
        {
            Title = title;
            Visibility = visibility;

            MainViewModel = mainViewModel;
            MainViewModel.TabRefresh += MainViewModel_TabRefresh;
        }

        /// <summary>
        /// Initialize the view when a <see cref="MainViewModel.TabRefresh"/> event
        /// makes the view visible.
        /// </summary>
        protected virtual void Initialize()
        { }

        /// <summary>
        /// Uninitialize the view when a <see cref="MainViewModel.TabRefresh"/> event
        /// hides the view.
        /// </summary>
        protected virtual void Shutdown()
        { }

        private void MainViewModel_TabRefresh(object sender, TabRefreshEventArgs e)
        {
            bool wasVisible = IsVisible;

            if (Visibility == TabPageVisibility.Always)
            {
                IsVisible = true;
            }
            else
            {
                switch (e.Trigger)
                {
                    case TabRefreshTrigger.WindowLoaded:
                    case TabRefreshTrigger.FileClosed:
                        IsVisible = (Visibility == TabPageVisibility.WhenFileIsClosed);
                        break;
                    case TabRefreshTrigger.FileOpened:
                        IsVisible = (Visibility == TabPageVisibility.WhenFileIsOpen);
                        Shutdown();
                        break;
                }
            }

            if (wasVisible)
            {
                Shutdown();
            }
            if (IsVisible)
            {
                Initialize();
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
        /// Indicates that a tab page should be visible only when not editing a file.
        /// </summary>
        WhenFileIsClosed
    }
}
