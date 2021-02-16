using System;
using System.ComponentModel;
using System.Windows.Controls;
using GTA3SaveEditor.GUI.Dialogs;
using WHampson.ToolUI;

namespace GTA3SaveEditor.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        private LogWindow m_logWindow;

        public new MainWindowVM ViewModel
        {
            get { return (MainWindowVM) DataContext; }
            set { DataContext = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            ViewModel.LogWindowRequest += ViewModel_LogWindowRequest;
            ViewModel.CustomScriptsDialogRequest += ViewModel_CustomScriptsDialogRequest;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel.IsDirty)
            {
                e.Cancel = true;
                ViewModel.ExitAppWithConfirmation();
                return;
            }

            base.OnClosing(e);
            DestroyAllWindows();
            ViewModel.LogWindowRequest -= ViewModel_LogWindowRequest;
        }

        private void LazyShowWindow<T>(T window, out T outWindow) where T : WindowBase, new()
        {
            if (window != null && window.IsVisible)
            {
                window.Focus();
                outWindow = window;
                return;
            }

            if (window == null)
            {
                window = new T() { Owner = this };
                window.ViewModel.OpenFileRequest += ViewModel.OpenFileRequest_Handler;
                window.ViewModel.SaveFileRequest += ViewModel.SaveFileRequest_Handler;
                window.ViewModel.CloseFileRequest += ViewModel.CloseFileRequest_Handler;
                window.ViewModel.RevertFileRequest += ViewModel.RevertFileRequest_Handler;
            }

            outWindow = window;
            window.Show();
        }

        private void DestroyWindow<T>(T window) where T : WindowBase
        {
            if (window != null)
            {
                window.HideOnClose = false;
                window.Close();
                window.ViewModel.OpenFileRequest -= ViewModel.OpenFileRequest_Handler;
                window.ViewModel.SaveFileRequest -= ViewModel.SaveFileRequest_Handler;
                window.ViewModel.CloseFileRequest -= ViewModel.CloseFileRequest_Handler;
                window.ViewModel.RevertFileRequest -= ViewModel.RevertFileRequest_Handler;
            }
        }

        private void DestroyAllWindows()
        {
            DestroyWindow(m_logWindow);

            m_logWindow = null;
        }

        public void ShowDialog<T>() where T : DialogBase, new()
        {
            T dialog = new T() { Owner = this };
            dialog.ShowDialog();
        }

        private void ViewModel_LogWindowRequest(object sender, EventArgs e)
        {
            LazyShowWindow(m_logWindow, out m_logWindow);
        }

        private void ViewModel_CustomScriptsDialogRequest(object sender, EventArgs e)
        {
            ShowDialog<CustomScriptsDialog>();
        }

        private void TabControlEx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl)
            {
                int i = ViewModel.SelectedTabIndex;
                if (i != -1) ViewModel.Tabs[i].Update();
            }
        }
    }
}
