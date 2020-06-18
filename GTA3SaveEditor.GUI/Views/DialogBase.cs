using GTA3SaveEditor.GUI.ViewModels;
using System.Windows;

namespace GTA3SaveEditor.GUI.Views
{
    public abstract class DialogBase<T> : Window
        where T : DialogViewModelBase
    {
        public T ViewModel
        {
            get { return (T) DataContext; }
            set { DataContext = value; }
        }

        protected DialogBase()
            : base()
        { }
    }
}
