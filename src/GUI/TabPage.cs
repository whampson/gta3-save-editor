using WHampson.ToolUI;

namespace GTA3SaveEditor.GUI
{
    public abstract class TabPage<T> : UserControlBase
        where T : TabPageVM
    {
        public new T ViewModel
        {
            get { return (T) DataContext; }
            set { DataContext = value; }
        }
    }
}
