using GTASaveData.GTA3;
using System;
using System.Collections.Generic;

namespace GTA3SaveEditor.GUI.Events
{
    public class RadarBlipEventArgs : EventArgs
    {
        public BlipAction Action { get; set; }
        public IList<RadarBlip> OldItems { get; set; }
        public IList<RadarBlip> NewItems { get; set; }
        public int OldStartingIndex { get; set; }
        public int NewStartingIndex { get; set; }

        public RadarBlipEventArgs()
        {
            OldStartingIndex = -1;
            NewStartingIndex = -1;
        }
    }

    public enum BlipAction
    {
        Add,
        Remove,
        Replace,
        Reset,
    }
}
