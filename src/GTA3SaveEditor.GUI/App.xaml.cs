using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Extensions;
using GTA3SaveEditor.Core.Util;
using GTASaveData;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += Application_UnhandledException;

            Log.InfoStream = LogWriter;
            Log.ErrorStream = LogWriter;

            string osVer = RuntimeInformation.OSDescription;
            string dotnetVer = RuntimeInformation.FrameworkDescription;
            string saveDataLibVer = Assembly.GetAssembly(typeof(SaveData)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            string gta3DataLibVer = Assembly.GetAssembly(typeof(GTA3Save)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            Log.Info($"{Name} {Version}");
            Log.Info($"{Copyright}");

            string sysInfoBanner = "System Info".CreateBanner('=', 15);
            string sysInfoBannerEnd = new string('=', sysInfoBanner.Length);
            Log.Info(sysInfoBanner);
            Log.Info($"   Operating System: {osVer}");
            Log.Info($"       .NET Runtime: {dotnetVer}");
            Log.Info($"   GTASaveData.Core: {saveDataLibVer}");
            Log.Info($"   GTASaveData.GTA3: {gta3DataLibVer}");
            Log.Info($"Save Editor Version: {AssemblyFileVersion}");
#if DEBUG
            Log.Info($"DEBUG build.");
#endif
#if RELEASE_STANDALONE
            //Settings.TheSettings.Updater.StandaloneRing = true;   // TODO
            Log.Info("Standalone build.");
#endif
            Log.Info(sysInfoBannerEnd);

            LoadResources();
            LoadSettings();

            TheWindow = new MainWindow() { Title = Name };
            TheWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SaveSettings();
            Log.Info("Exiting...");

            if (SaveEditor.Settings.WriteLogFile)
            {
                string logFilePath = SaveEditor.Settings.LogFilePath;
                if (string.IsNullOrEmpty(logFilePath))
                {
                    logFilePath = $"{ShortName}_{DateTime.Now:yyyyMMddHHmmss}.log";
                }
                File.WriteAllText(logFilePath, LogText);
            }
        }

        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Log.Error(ex);

            if (Debugger.IsAttached)
            {
                // Let the debugger handle the exception
                return;
            }

            Log.Info($"A catastrophic error has occurred. Please report this issue to {AuthorContact}.");

            string logFile = $"crash-dump_{DateTime.Now:yyyyMMddHHmmss}.log";
            File.WriteAllText(logFile, LogText);

            TheWindow.ViewModel.ShowError(
                $"An unhandled exception has occurred. The program will close and you will lose all unsaved changes.\n" +
                $"\n" +
                $"{ex.GetType().Name}: {ex.Message}\n" +
                $"\n" +
                $"A log file has been created: {logFile}. " +
                $"Please report this issue to {AuthorContact} and include this log file with your report.",
                "Unhandled Exception");

            Environment.Exit(1);
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
