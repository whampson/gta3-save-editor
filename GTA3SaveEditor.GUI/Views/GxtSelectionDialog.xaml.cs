using GTA3SaveEditor.Core;
using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for GxtSelectionDialog.xaml
    /// </summary>
    public partial class GxtSelectionDialog : DialogBase<GxtSelector>
    {
        public Gxt GxtTable
        {
            get { return ViewModel.GxtTable; }
            set { ViewModel.GxtTable = value; }
        }

        public KeyValuePair<string, string> SelectedItem
        {
            get { return ViewModel.SelectedItem; }
        }

        public GxtSelectionDialog()
        {
            InitializeComponent();
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.DialogCloseRequest += ViewModel_DialogCloseRequest;
        }

        private void Dialog_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.DialogCloseRequest -= ViewModel_DialogCloseRequest;
        }

        private void ViewModel_DialogCloseRequest(object sender, DialogCloseEventArgs e)
        {
            DialogResult = e.DialogResult;
            Close();
        }
    }
}
