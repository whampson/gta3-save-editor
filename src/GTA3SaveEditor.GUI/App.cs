using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Util;
using GTASaveData;
using GTASaveData.GTA3;

namespace GTA3SaveEditor.GUI
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;

            Log.InfoStream = LogWriter;
            Log.ErrorStream = LogWriter;

            string osVer = RuntimeInformation.OSDescription;
            string dotnetVer = RuntimeInformation.FrameworkDescription;
            string saveDataLibVer = Assembly.GetAssembly(typeof(SaveData)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            string gta3DataLibVer = Assembly.GetAssembly(typeof(GTA3Save)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            Log.Info($"{Name} {VersionString}");
            Log.Info($"{Copyright}");
            Log.Info(new string('=', 40));
            Log.Info($"Operating System: {osVer}");
            Log.Info($"    .NET Runtime: {dotnetVer}");
            Log.Info($"GTASaveData.Core: {saveDataLibVer}");
            Log.Info($"GTASaveData.GTA3: {gta3DataLibVer}");
            Log.Info($"  Editor Version: {AssemblyFileVersion}");
#if DEBUG
            Log.Info($"DEBUG build.");
#endif
#if RELEASE_STANDALONE
            IsStandaloneBuild = true;
            Log.Info("Standalone build.");
#endif

            LoadResources();
            LoadSettings();

            TheWindow = new MainWindow() { Title = Name };
            TheWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
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

        private void LoadResources()
        {
            Log.Info("Loading resources...");
            SaveEditor.CarColors = CarColorsLoader.LoadColors(LoadResource("carcols.dat"));
            SaveEditor.GxtTable = GxtLoader.Load(LoadResource("english.gxt"));
            SaveEditor.IdeObjects = IdeLoader.LoadObjects(LoadResource("gta3.ide"));
        }

        private void LoadSettings()
        {
            Log.Info("Loading settings...");
            SaveEditor.LoadSettings(SettingsPath);
        }

        private void SaveSettings()
        {
            Log.Info("Saving settings...");
            SaveEditor.SaveSettings(SettingsPath);
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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

            if (TheWindow != null)
            {
                TheWindow.ViewModel.ShowError(
                    $"An unhandled exception has occurred. The program will close and you will lose all unsaved changes.\n" +
                    $"\n" +
                    $"{ex.GetType().Name}: {ex.Message}\n" +
                    $"\n" +
                    $"A log file has been created: {logFile}. " +
                    $"Please report this issue to {AuthorContact} and include this log file with your report.",
                    "Unhandled Exception");
            }

            Environment.Exit(1);
        }
    }
}
