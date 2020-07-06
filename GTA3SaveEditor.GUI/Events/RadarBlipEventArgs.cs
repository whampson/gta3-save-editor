using GTASaveData.GTA3;
using System;
using System.Collections.Generic;

namespace GTA3SaveEditor.GUI.Events
{
    public class RadarBlipEventArgs : EventArgs
    {
        public BlipAction Action { get; set; }
        public IList<RadarBlip> Items { get; set; }

        public RadarBlipEventArgs()
        {
            Items = new List<RadarBlip>();
        }
    }

    public enum BlipAction
    {
        Update,
        Reset,
    }
}
