using GTA3SaveEditor.GUI.Events;
using System;

namespace GTA3SaveEditor.GUI.ViewModels
{
    /// <summary>
    /// The view model base for tab pages.
    /// </summary>
    public abstract class TabPageViewModelBase : ViewModelBase
    {
        public event EventHandler ShuttingDown;

        private bool m_isVisible;

        /// <summary>
        /// Gets or sets whether the tab page is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return m_isVisible; }
            set
            {
                bool wasVisible = m_isVisible;
                m_isVisible = value;

                if (wasVisible && !m_isVisible) Unload();
                if (m_isVisible && !wasVisible) Load();
                OnPropertyChanged();
            }
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
        /// Gets the main window data context for accessing global functions.
        /// </summary>
        public Main MainViewModel { get; }

        /// <summary>
        /// Creates a new <see cref="TabPageViewModelBase"/> instance.
        /// </summary>
        /// <param name="title">The tab name.</param>
        /// <param name="visibility">The tab visibility setting.</param>
        /// <param name="mainViewModel">The main window data context.</param>
        public TabPageViewModelBase(string title, TabPageVisibility visibility, Main mainViewModel)
        {
            Title = title;
            Visibility = visibility;
            MainViewModel = mainViewModel;
        }

        public virtual void Initialize()
        {
            MainViewModel.TabUpdate += MainViewModel_TabUpdate;
        }

        public virtual void Shutdown()
        {
            MainViewModel.TabUpdate -= MainViewModel_TabUpdate;
            ShuttingDown?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Load()
        { }

        public virtual void Unload()
        { }

        public virtual void Refresh()
        { }

        /// <summary>
        /// Handle TabUpdate events from the main view model.
        /// </summary>
        private void MainViewModel_TabUpdate(object sender, TabUpdateEventArgs e)
        {
            switch (e.Trigger)
            {
                case TabUpdateTrigger.FileOpened:
                    IsVisible = (Visibility == TabPageVisibility.Always) || (Visibility == TabPageVisibility.WhenFileIsOpen);
                    break;
                case TabUpdateTrigger.FileClosing:
                case TabUpdateTrigger.WindowLoaded:
                    IsVisible = (Visibility == TabPageVisibility.Always) || (Visibility == TabPageVisibility.WhenFileIsClosed);
                    break;
                case TabUpdateTrigger.WindowClosing:
                    IsVisible = false;
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
        /// Indicates that a tab page should be visible only when not editing a file.
        /// </summary>
        WhenFileIsClosed
    }
}
