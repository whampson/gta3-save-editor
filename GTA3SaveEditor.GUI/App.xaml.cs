﻿using GTA3SaveEditor.GUI.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTA3SaveEditor.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string SettingsPath = "settings.json";

        private MainWindow TheWindow { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += Application_UnhandledException;

            TheWindow = new MainWindow
            {
                Height = 600,
                Width = 800
            };

            TheWindow.Show();
        }

        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if !DEBUG
            ShowUnhandledExceptionMessage(e.ExceptionObject as Exception);
#endif
            // Let VisualStudio handle the exception otherwise
        }

        private void ShowUnhandledExceptionMessage(Exception e)
        {
            string text = $"An unhandled exception has occurred.";
            if (e != null)
            {
                text += $"\n\n{ e.GetType().Name}: { e.Message}";
                if (e.StackTrace != null)
                {
                    text += $"\n\nStack trace:\n{e.StackTrace}\n";
                }
            }
            
            TheWindow.ViewModel.ShowMessageBoxError(text, "Unhandled Exception");
            // TODO: log file?
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }
    }
}
