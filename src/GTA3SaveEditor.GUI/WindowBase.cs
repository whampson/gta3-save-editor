namespace GTA3SaveEditor.GUI
{
    public class WindowBase : WHampson.ToolUI.WindowBase
    {
        public new WindowVMBase ViewModel
        {
            get { return (WindowVMBase) DataContext; }
            set { DataContext = value; }
        }
    }
}
