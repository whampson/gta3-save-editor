using System;

namespace GTA3SaveEditor.GUI.Events
{
    public class FileIOEventArgs : EventArgs
    {
        public string Path { get; set; }
        public bool SuppressDialogs { get; set; }
    }
}
