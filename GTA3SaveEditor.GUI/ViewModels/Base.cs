using GTA3SaveEditor.GUI.Events;
using System;
using System.Windows.Input;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    { }

    /// <summary>
    /// The view model base for tab pages.
    /// </summary>
    public abstract class TabPageViewModelBase : ViewModelBase
    {
        public event EventHandler Initializing;
        public event EventHandler Loading;
        public event EventHandler Unloading;
        public event EventHandler ShuttingDown;
        public event EventHandler Updating;

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

        /// <summary>
        /// Called when the tab page is created.
        /// </summary>
        public virtual void Initialize()
        {
            MainViewModel.TabUpdate += MainViewModel_TabUpdate;
            Initializing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the tab page is being destroyed.
        /// </summary>
        public virtual void Shutdown()
        {
            MainViewModel.TabUpdate -= MainViewModel_TabUpdate;
            ShuttingDown?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the tab page's content is loading.
        /// </summary>
        public virtual void Load()
        {
            Loading?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the tab page's content is unloading.
        /// </summary>
        public virtual void Unload()
        {
            Unloading?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the tab page's content is updating.
        /// </summary>
        public virtual void Update()
        {
            Updating?.Invoke(this, EventArgs.Empty);
        }

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

    public abstract class DialogViewModelBase : ViewModelBase
    {
        public event EventHandler<DialogCloseEventArgs> DialogCloseRequest;

        public DialogViewModelBase()
            : base()
        { }

        public void CloseDialog(bool? result = null)
        {
            DialogCloseRequest?.Invoke(this, new DialogCloseEventArgs(result));
        }

        public ICommand CloseCommand => new RelayCommand<bool?>
        (
            (result) => CloseDialog(result)
        );

        public ICommand CancelCommand => new RelayCommand
        (
            () => CloseDialog(false)
        );
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
