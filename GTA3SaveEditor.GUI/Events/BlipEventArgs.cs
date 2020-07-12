using GTASaveData.GTA3;
using System;
using System.Collections.Generic;

namespace GTA3SaveEditor.GUI.Events
{
    public class BlipEventArgs<T> : EventArgs
    {
        public BlipAction Action { get; set; }
        public IList<T> Items { get; set; }

        public BlipEventArgs()
        {
            Items = new List<T>();
        }
    }

    public enum BlipAction
    {
        Update,
        Reset,
    }
}
