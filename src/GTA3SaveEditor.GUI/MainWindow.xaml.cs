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
            RegisterHandlers(this);
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
            UnregisterHandlers(this);
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
                RegisterHandlers(window);
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
                UnregisterHandlers(window);
            }
        }

        private void DestroyAllWindows()
        {
            DestroyWindow(m_logWindow);

            m_logWindow = null;
        }

        public void ShowDialog<T>() where T : WindowBase, new()
        {
            T dialog = new T() { Owner = this };
            dialog.ShowDialog();
        }

        private void RegisterHandlers(WindowBase w)
        {
            w.ViewModel.LogWindowRequest += ViewModel_LogWindowRequest;
            w.ViewModel.CustomScriptsDialogRequest += ViewModel_CustomScriptsDialogRequest;
            w.ViewModel.OpenFileRequest += ViewModel.OpenFileRequest_Handler;
            w.ViewModel.SaveFileRequest += ViewModel.SaveFileRequest_Handler;
            w.ViewModel.CloseFileRequest += ViewModel.CloseFileRequest_Handler;
            w.ViewModel.RevertFileRequest += ViewModel.RevertFileRequest_Handler;
            w.ViewModel.InputDialogRequest += ViewModel_InputDialogRequest;

            // TODO: close handler
        }

        private void UnregisterHandlers(WindowBase w)
        {
            w.ViewModel.LogWindowRequest -= ViewModel_LogWindowRequest;
            w.ViewModel.CustomScriptsDialogRequest -= ViewModel_CustomScriptsDialogRequest;
            w.ViewModel.OpenFileRequest -= ViewModel.OpenFileRequest_Handler;
            w.ViewModel.SaveFileRequest -= ViewModel.SaveFileRequest_Handler;
            w.ViewModel.CloseFileRequest -= ViewModel.CloseFileRequest_Handler;
            w.ViewModel.RevertFileRequest -= ViewModel.RevertFileRequest_Handler;
            w.ViewModel.InputDialogRequest -= ViewModel_InputDialogRequest;
        }

        private void ViewModel_LogWindowRequest(object sender, EventArgs e)
        {
            LazyShowWindow(m_logWindow, out m_logWindow);
        }

        private void ViewModel_CustomScriptsDialogRequest(object sender, EventArgs e)
        {
            ShowDialog<CustomScriptsDialog>();
        }

        private void ViewModel_InputDialogRequest(object sender, Events.InputDialogEventArgs e)
        {
            var vm = new InputDialogVM()
            {
                Title = e.Title ?? "Input Required",
                Label = e.Label ?? "Enter Input:",
            };

            InputDialog d = new InputDialog()
            {
                Owner = this,
                ViewModel = vm
            };
            d.ShowDialog();

            e.Result = vm.Text;
            e.Callback?.Invoke(e);
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
