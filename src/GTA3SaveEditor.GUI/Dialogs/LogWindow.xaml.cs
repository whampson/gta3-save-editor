using GTA3SaveEditor.Core.Util;
using System;
using System.ComponentModel;

namespace GTA3SaveEditor.GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : WindowBase
    {
        public new LogWindowVM ViewModel
        {
            get { return (LogWindowVM) DataContext; }
            set { DataContext = value; }
        }

        public LogWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            UpdateTextBox();
            Log.LogEvent += Log_LogEvent;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Log.LogEvent -= Log_LogEvent;
        }

        private void UpdateTextBox()
        {
            Dispatcher.Invoke(() =>
            {
                m_textBox.Text = App.LogText;
                m_textBox.ScrollToEnd();
            });
        }

        private void Log_LogEvent(object sender, EventArgs e)
        {
            UpdateTextBox();
        }
    }
}
