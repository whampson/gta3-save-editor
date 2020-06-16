using GTA3SaveEditor.GUI.ViewModels;
using System.Windows.Input;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : TabPageViewBase
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        public WelcomeViewModel ViewModel
        {
            get { return (WelcomeViewModel) DataContext; }
            set { DataContext = value; }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedFile != null)
            {
                ViewModel.LoadCommand.Execute(null);
            }
        }
    }
}
