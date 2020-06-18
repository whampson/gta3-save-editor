using GTA3SaveEditor.GUI.Events;

namespace GTA3SaveEditor.GUI.ViewModels
{
    /// <summary>
    /// The view model base for tab pages.
    /// </summary>
    public abstract class TabPageViewModelBase : ViewModelBase
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
        /// be visible or hidden when a <see cref="Main.TabUpdate"/> event occurs.
        /// </summary>
        public TabPageVisibility Visibility { get; }

        /// <summary>
        /// Gets the tab name.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the main window view model for accessing global functions.
        /// </summary>
        public Main MainWindow { get; }

        public TabPageViewModelBase(string title, TabPageVisibility visibility, Main mainWindow)
        {
            Title = title;
            Visibility = visibility;

            MainWindow = mainWindow;
            MainWindow.TabUpdate += MainViewModel_TabUpdate;
        }

        /// <summary>
        /// Initializes the view when a <see cref="Main.TabUpdate"/> event
        /// makes the view visible.
        /// </summary>
        protected virtual void Initialize()
        { }

        /// <summary>
        /// Uninitializes the view when a <see cref="Main.TabUpdate"/> event
        /// hides the view.
        /// </summary>
        protected virtual void Shutdown()
        { }

        /// <summary>
        /// Refreshes the view content.
        /// </summary>
        public virtual void Refresh()
        { }

        private void MainViewModel_TabUpdate(object sender, TabUpdateEventArgs e)
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
                    case TabUpdateTrigger.WindowLoaded:
                    case TabUpdateTrigger.FileClosed:
                        IsVisible = (Visibility == TabPageVisibility.WhenFileIsClosed);
                        break;
                    case TabUpdateTrigger.FileOpened:
                        IsVisible = (Visibility == TabPageVisibility.WhenFileIsOpen);
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
