using System;

namespace GTA3SaveEditor.GUI.Events
{
    public class DialogCloseEventArgs : EventArgs
    {
        public bool? DialogResult { get; set; }

        public DialogCloseEventArgs(bool? dialogResult = null)
        {
            DialogResult = dialogResult;
        }
    }
}
