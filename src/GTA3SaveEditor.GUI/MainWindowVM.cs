using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WHampson.ToolUI;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI
{
    public class MainWindowVM : WindowVM
    {
        public event EventHandler LogWindowRequest;

        public ICommand ViewLogCommand => new RelayCommand
        (
            () => LogWindowRequest?.Invoke(this, EventArgs.Empty)
        );
    }
}
