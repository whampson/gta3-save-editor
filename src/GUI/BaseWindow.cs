namespace GTA3SaveEditor.GUI
{
    public class BaseWindow : WHampson.ToolUI.WindowBase
    {
        public new BaseWindowVM ViewModel
        {
            get { return (BaseWindowVM) DataContext; }
            set { DataContext = value; }
        }
    }
}
