namespace GTA3SaveEditor.GUI.ViewModels
{
    public class GeneralViewModel : TabPageViewModelBase
    {
        public GeneralViewModel(MainViewModel mainViewModel)
            : base("General", TabPageVisibility.WhenFileIsOpen, mainViewModel)
        { }
    }
}
