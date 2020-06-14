using System;

namespace GTA3SaveEditor.GUI.Events
{
    /// <summary>
    /// Parameters for handling a tab refresh event.
    /// </summary>
    public class TabRefreshEventArgs : EventArgs
    {
        public TabRefreshEventArgs(TabRefreshTrigger trigger)
        {
            Trigger = trigger;
        }

        public TabRefreshTrigger Trigger
        {
            get;
        }
    }

    /// <summary>
    /// Tab refresh trigger event types.
    /// </summary>
    public enum TabRefreshTrigger
    {
        /// <summary>
        /// Refresh occurred after the window finished loading.
        /// </summary>
        WindowLoaded,

        /// <summary>
        /// Refresh occurred after a file was opened.
        /// </summary>
        FileOpened,

        /// <summary>
        /// Refresh occurred after a file was closed.
        /// </summary>
        FileClosed
    }
}
