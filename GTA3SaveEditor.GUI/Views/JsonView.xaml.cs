using GTA3SaveEditor.GUI.ViewModels;
using System.Windows.Controls;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for JsonView.xaml
    /// </summary>
    public partial class JsonView : TabPageBase<JsonViewer>
    {
        public JsonView()
        {
            InitializeComponent();
        }

        private void BlockComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.UpdateTextBox();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel.SelectedBlockIndex == 0)
            {
                ViewModel.UpdateTextBox();
            }
        }
    }
}
