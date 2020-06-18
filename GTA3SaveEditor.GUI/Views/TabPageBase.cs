using GTA3SaveEditor.GUI.ViewModels;
using System.Windows.Controls;

namespace GTA3SaveEditor.GUI.Views
{
    public abstract class TabPageBase<T> : UserControl
        where T : TabPageViewModelBase
    {
        public T ViewModel
        {
            get { return (T) DataContext; }
            set { DataContext = value; }
        }

        protected TabPageBase()
            : base()
        { }
    }
}
