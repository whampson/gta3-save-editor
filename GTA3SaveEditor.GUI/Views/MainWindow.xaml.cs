using GTA3SaveEditor.GUI.Events;
using GTA3SaveEditor.GUI.ViewModels;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_isInitializing;

        public Main ViewModel
        {
            get { return (Main) DataContext; }
            set { DataContext = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel.MessageBoxRequest += ViewModel_MessageBoxRequested;
            ViewModel.FileDialogRequest += ViewModel_FileDialogRequested;
            ViewModel.FolderDialogRequest += ViewModel_FolderDialogRequested;
            ViewModel.GxtSelectionDialogRequest += ViewModel_GxtSelectionDialogRequest;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_isInitializing = true;
            ViewModel.Initialize();
            m_isInitializing = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.Shutdown();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_isInitializing || !(e.OriginalSource is TabControl))
            {
                return;
            }

            object tabPage = e.AddedItems[0];
            if (tabPage is TabPageViewModelBase page)
            {
                page.Refresh();
            }
        }

        private void ViewModel_MessageBoxRequested(object sender, MessageBoxEventArgs e)
        {
            e.Show(this);
        }

        private void ViewModel_FileDialogRequested(object sender, FileDialogEventArgs e)
        {
            e.ShowDialog(this);
        }

        private void ViewModel_FolderDialogRequested(object sender, FileDialogEventArgs e)
        {
            VistaFolderBrowserDialog d = new VistaFolderBrowserDialog();
            bool? r = d.ShowDialog(this);

            e.FileName = d.SelectedPath;
            e.Callback?.Invoke(r, e);
        }

        private void ViewModel_GxtSelectionDialogRequest(object sender, GxtSelectionEventArgs e)
        {
            GxtSelectionDialog d = new GxtSelectionDialog()
            {
                Owner = this,
                GxtTable = e.GxtTable
            };
            bool? r = d.ShowDialog();

            e.SelectedKey = d.SelectedItem.Key;
            e.SelectedValue = d.SelectedItem.Value;
            e.Callback?.Invoke(r, e);
        }
    }
}
