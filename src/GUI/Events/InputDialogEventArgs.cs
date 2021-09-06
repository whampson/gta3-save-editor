using System;
using System.Collections.Generic;
using System.Text;

namespace GTA3SaveEditor.GUI.Events
{
    public class InputDialogEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Label { get; set; }
        public string Result { get; set; }
        public Action<InputDialogEventArgs> Callback { get; set; }
    }
}
