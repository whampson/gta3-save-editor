using GTA3SaveEditor.GUI.ViewModels;
using System.Windows.Input;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : TabPageBase<Welcome>
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.LoadCommand.Execute(null);
            }
        }
    }
}
