using GTA3SaveEditor.GUI.ViewModels;
using System.Windows;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralView : TabPageBase<General>
    {
        public GeneralView()
        {
            InitializeComponent();
        }

        private void GxtCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Trigger update
            ViewModel.SaveTitleGxtKey = ViewModel.SaveTitleGxtKey;
        }

        private void GxtCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Trigger update
            ViewModel.SaveTitle = ViewModel.SaveTitle;
        }
    }
}
