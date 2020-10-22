﻿using System;
using System.ComponentModel;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Util;
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
            Log.Info("Ready.");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // TODO
            //if (ViewModel.IsDirty)
            //{
            //    e.Cancel = true;
            //    ViewModel.PromptSaveChanges(ExitAppDialog_Callback);
            //    return;
            //}

            base.OnClosing(e);
            DestroyAllWindows();
            ViewModel.LogWindowRequest -= ViewModel_LogWindowRequest;
        }

        private void LazyLoadWindow<T>(T window, out T outWindow) where T : WindowBase, new()
        {
            if (window != null && window.IsVisible)
            {
                window.Focus();
                outWindow = window;
                return;
            }


            if (window == null)
            {
                // TODO
                window = new T() { Owner = this };
                //window.ViewModel.OpenFileRequest += ViewModel.OpenFileRequest_Handler;
                //window.ViewModel.SaveFileRequest += ViewModel.SaveFileRequest_Handler;
                //window.ViewModel.CloseFileRequest += ViewModel.CloseFileRequest_Handler;
                //window.ViewModel.RevertFileRequest += ViewModel.RevertFileRequest_Handler;
            }

            outWindow = window;
            window.Show();
        }

        private void DestroyWindow<T>(T window) where T : WindowBase
        {
            if (window != null)
            {
                // TODO
                window.HideOnClose = false;
                window.Close();
                //window.ViewModel.OpenFileRequest -= ViewModel.OpenFileRequest_Handler;
                //window.ViewModel.SaveFileRequest -= ViewModel.SaveFileRequest_Handler;
                //window.ViewModel.CloseFileRequest -= ViewModel.CloseFileRequest_Handler;
                //window.ViewModel.RevertFileRequest -= ViewModel.RevertFileRequest_Handler;
            }
        }

        private void DestroyAllWindows()
        {
            DestroyWindow(m_logWindow);

            m_logWindow = null;
        }

        private void ViewModel_LogWindowRequest(object sender, EventArgs e)
        {
            LazyLoadWindow(m_logWindow, out m_logWindow);
        }
    }
}