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
        private bool m_initialized;
        private bool m_initializing;
        
        public Main ViewModel
        {
            get { return (Main) DataContext; }
            set { DataContext = value; }
        }

        public MainWindow()
        {
            m_initializing = true;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_initialized) return;

            ViewModel.Initialize();
            ViewModel.MessageBoxRequest += ViewModel_MessageBoxRequest;
            ViewModel.FileDialogRequest += ViewModel_FileDialogRequest;
            ViewModel.FolderDialogRequest += ViewModel_FolderDialogRequest;
            ViewModel.GxtSelectionDialogRequest += ViewModel_GxtSelectionDialogRequest;

            m_initializing = false;
            m_initialized = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (ViewModel.IsDirty)
            {
                e.Cancel = true;
                ViewModel.ShowSaveConfirmationDialog(ExitAppConfirmationDialog_Callback);
                return;
            }

            ViewModel.Shutdown();
            ViewModel.MessageBoxRequest -= ViewModel_MessageBoxRequest;
            ViewModel.FileDialogRequest -= ViewModel_FileDialogRequest;
            ViewModel.FolderDialogRequest -= ViewModel_FolderDialogRequest;
            ViewModel.GxtSelectionDialogRequest -= ViewModel_GxtSelectionDialogRequest;

            m_initialized = false;
        }

        private void ExitAppConfirmationDialog_Callback(MessageBoxResult r)
        {
            if (r != MessageBoxResult.Cancel)
            {
                if (r == MessageBoxResult.Yes) ViewModel.SaveFile();
                ViewModel.IsDirty = false;
                ViewModel.CloseFile();
                Application.Current.Shutdown();
            }
        }

        private void ViewModel_MessageBoxRequest(object sender, MessageBoxEventArgs e)
        {
            e.Show(this);
        }

        private void ViewModel_FileDialogRequest(object sender, FileDialogEventArgs e)
        {
            e.ShowDialog(this);
        }

        private void ViewModel_FolderDialogRequest(object sender, FileDialogEventArgs e)
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


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_initializing || !(e.OriginalSource is TabControl))
            {
                return;
            }

            foreach (var item in e.AddedItems)
            {
                if (item is TabPageViewModelBase tabPageViewModel)
                {
                    tabPageViewModel.Update();
                }
            }
        }
    }
}
