using System;

namespace GTA3SaveEditor.GUI
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
}
