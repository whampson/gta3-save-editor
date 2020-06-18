using System;

namespace GTA3SaveEditor.GUI.Events
{
    /// <summary>
    /// Parameters for handling a tab update event.
    /// </summary>
    public class TabUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The event trigger type.
        /// </summary>
        public TabUpdateTrigger Trigger { get; }

        public TabUpdateEventArgs(TabUpdateTrigger trigger)
        {
            Trigger = trigger;
        }
    }

    /// <summary>
    /// Tab update trigger event types.
    /// </summary>
    public enum TabUpdateTrigger
    {
        /// <summary>
        /// Update occurred as a result of the window loading.
        /// </summary>
        WindowLoaded,

        /// <summary>
        /// Update occurred as a result of a file being opened.
        /// </summary>
        FileOpened,

        /// <summary>
        /// Update occurred as a result a file being closed.
        /// </summary>
        FileClosed
    }
}
