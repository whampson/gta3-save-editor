using GTA3SaveEditor.GUI.Events;
using System;
using System.Windows.Input;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public abstract class DialogViewModelBase : ViewModelBase
    {
        public event EventHandler<DialogCloseEventArgs> DialogCloseRequest;

        public DialogViewModelBase()
            : base()
        { }

        public void CloseDialog(bool? result = null)
        {
            DialogCloseRequest?.Invoke(this, new DialogCloseEventArgs(result));
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand<bool?>
                (
                    (result) => CloseDialog(result)
                );
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => CloseDialog(false)
                );
            }
        }
    }
}
