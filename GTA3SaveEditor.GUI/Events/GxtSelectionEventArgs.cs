using GTA3SaveEditor.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTA3SaveEditor.GUI.Events
{
    public class GxtSelectionEventArgs : EventArgs
    {
        public Gxt GxtTable { get; set; }
        public string SelectedKey { get; set; }
        public string SelectedValue { get; set; }
        public  Action<bool?, GxtSelectionEventArgs> Callback { get; set; }

        public GxtSelectionEventArgs()
        { }
    }
}
