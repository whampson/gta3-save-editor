using GTA3SaveEditor.GUI.ViewModels;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Windows;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel.FolderDialogRequested += ViewModel_FolderDialogRequested;
            ViewModel.FileDialogRequested += ViewModel_FileDialogRequested;
            ViewModel.MessageBoxRequested += ViewModel_MessageBoxRequested;
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel) DataContext; }
            set { DataContext = value; }
        }

        private void ViewModel_FolderDialogRequested(object sender, FileDialogEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            bool? result = dialog.ShowDialog(this);

            e.FileName = dialog.SelectedPath;
            e.Callback?.Invoke(result, e);
        }

        private void ViewModel_FileDialogRequested(object sender, FileDialogEventArgs e)
        {
            e.ShowDialog(this);
        }

        private void ViewModel_MessageBoxRequested(object sender, MessageBoxEventArgs e)
        {
            e.Show(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.Shutdown();
        }
    }
}
