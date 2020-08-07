using GTA3SaveEditor.Core;
using System;
using System.Windows;
using System.Windows.Threading;
using WpfEssentials;
using WpfEssentials.Win32;

namespace GTA3SaveEditor.GUI.ViewModels
{
    public abstract class BaseWindow : ObservableObject
    {
        public event EventHandler<MessageBoxEventArgs> MessageBoxRequest;
        public event EventHandler<FileDialogEventArgs> FileDialogRequest;
        public event EventHandler<FileDialogEventArgs> FolderDialogRequest;

        private readonly DispatcherTimer m_statusTextTimer;

        private string m_title;
        private string m_statusText;
        private string m_defaultStatusText;
        private string m_timedStatusText;
        private int m_timedStatusTextTime;
        private int m_timedStatusTextTick;

        public string Title
        {
            get { return m_title; }
            set { m_title = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get { return m_statusText; }
            set { m_statusText = value; OnPropertyChanged(); }
        }

        public string DefaultStatusText
        {
            get { return m_defaultStatusText; }
            set { m_defaultStatusText = value; OnPropertyChanged(); }
        }

        public string TimedStatusText
        {
            get { return m_timedStatusText; }
            set
            {
                m_timedStatusText = value;
                m_timedStatusTextTick = TimedStatusTextTime;
                m_statusTextTimer.Interval = TimeSpan.Zero;
                m_statusTextTimer.Start();
                OnPropertyChanged();
            }
        }

        public int TimedStatusTextTime
        {
            get { return m_timedStatusTextTime; }
            set { m_timedStatusTextTime = value; OnPropertyChanged(); }
        }

        public BaseWindow()
        {
            m_statusTextTimer = new DispatcherTimer();
        }

        public virtual void Initialize()
        {
            m_statusTextTimer.Tick += StatusTimer_Tick;
     
        }

        public virtual void Shutdown()
        {
            m_statusTextTimer.Tick -= StatusTimer_Tick;
        }

        public void ShowMessageBoxInfo(string text, string title = "Information")
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Information));
        }

        public void ShowMessageBoxWarning(string text, string title = "Warning")
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Warning));
        }

        public void ShowMessageBoxError(string text, string title = "Error")
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                text, title, icon: MessageBoxImage.Error));
        }

        public void ShowMessageBoxException(Exception e, string text = "An error has occurred.", string title = "Error")
        {
            text += $"\n\n{e.GetType().Name}: {e.Message}";
            ShowMessageBoxError(text, title);
        }

        public void ShowSaveYesNoCancelDialog(Action<MessageBoxResult> callback)
        {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(
                "Do you want to save your changes?", "Save",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question, callback: callback));
        }

        public void ShowFileDialog(FileDialogType type, Action<bool?, FileDialogEventArgs> callback)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, callback)
            {
                InitialDirectory = Settings.TheSettings.MostRecentFile
            };
            FileDialogRequest?.Invoke(this, e);
        }

        public void ShowFolderDialog(FileDialogType type, Action<bool?, FileDialogEventArgs> callback)
        {
            FileDialogEventArgs e = new FileDialogEventArgs(type, callback)
            {
                InitialDirectory = Settings.TheSettings.MostRecentFile
            };
            FolderDialogRequest?.Invoke(this, e);
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            if (m_timedStatusTextTick == TimedStatusTextTime)
            {
                m_statusTextTimer.Interval = TimeSpan.FromSeconds(1);
            }
            if (m_timedStatusTextTick <= 0)
            {
                m_statusTextTimer.Stop();
                StatusText = DefaultStatusText;
            }
            else
            {
                StatusText = TimedStatusText;
                m_timedStatusTextTick--;
            }
        }
    }
}
