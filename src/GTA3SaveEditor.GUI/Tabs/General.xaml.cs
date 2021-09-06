using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTA3SaveEditor.GUI.Tabs
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class GeneralTab : TabPage<GeneralVM>
    {
        public GeneralTab()
        {
            InitializeComponent();
        }

        private void GxtStringCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null || ViewModel.IsUpdating)
            {
                return;
            }

            ViewModel.UpdateName(true);
            ViewModel.UpdateNameTextBoxVisibility();
        }

        private void GxtStringCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null || ViewModel.IsUpdating)
            {
                return;
            }

            ViewModel.UpdateName(false);
            ViewModel.UpdateNameTextBoxVisibility();
        }
    }
}
