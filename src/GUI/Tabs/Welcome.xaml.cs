using System.Windows.Input;

namespace GTA3SaveEditor.GUI.Tabs
{
    /// <summary>
    /// Interaction logic for WelcomeTab.xaml
    /// </summary>
    public partial class WelcomeTab : TabPage<WelcomeVM>
    {
        public WelcomeTab()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedFile != null)
            {
                ViewModel.OpenSelectedItem();
            }
        }
    }
}
