using System.Windows;
using System.Windows.Controls;
using GTA3SaveEditor.Core.Game;
using WHampson.ToolUI;

namespace GTA3SaveEditor.GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomScriptsDialog.xaml
    /// </summary>
    public partial class CustomScriptsDialog : DialogBase
    {
        public new CustomScriptsVM ViewModel
        {
            get { return (CustomScriptsVM) DataContext; }
            set { DataContext = value; }
        }

        public CustomScriptsDialog()
        {
            InitializeComponent();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel.SelectedScript is CustomScript sc)
            {
                string name = sc.Code.Name;
                if (name != sc.Thread.Name)
                {
                    ViewModel.ShouldCommit = true;
                    sc.Thread.Name = name;
                }
            }
        }
    }
}
