﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            Log.DebugStream = LogWriter;

            string osVer = RuntimeInformation.OSDescription;
            string dotnetVer = RuntimeInformation.FrameworkDescription;
            string saveDataLibVer = Assembly.GetAssembly(typeof(SaveFile)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            string gta3DataLibVer = Assembly.GetAssembly(typeof(SaveFileGTA3)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

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

            TheWindow = new MainWindow();
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
            List<IdeObject> objects = IdeLoader.LoadObjects(LoadResource("gta3.ide"));
            objects.Add(new IdeObject(0, ""));
            objects.Add(new IdeObject(170, "grenade"));
            objects.Add(new IdeObject(171, "ak47"));
            objects.Add(new IdeObject(172, "bat"));
            objects.Add(new IdeObject(173, "colt45"));
            objects.Add(new IdeObject(174, "molotov"));
            objects.Add(new IdeObject(175, "rocket"));
            objects.Add(new IdeObject(176, "shotgun"));
            objects.Add(new IdeObject(177, "sniper"));
            objects.Add(new IdeObject(178, "uzi"));
            objects.Add(new IdeObject(179, "missile"));
            objects.Add(new IdeObject(180, "m16"));
            objects.Add(new IdeObject(181, "flame"));
            objects.Add(new IdeObject(182, "bomb"));
            objects.Add(new IdeObject(183, "fingers"));

            SaveEditor.IdeObjects = objects.OrderBy(x => x.ModelName).ToList();
            SaveEditor.CarColors = CarColorsLoader.LoadColors(LoadResource("carcols.dat"));
            SaveEditor.GxtTable = GxtLoader.Load(LoadResource("english.gxt"));
        }

        private void LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                Log.Info("Settings file not found. Creating new settings file...");
                SaveEditor.SaveSettings(SettingsPath);
                return;
            }

            SaveEditor.LoadSettings(SettingsPath);
        }

        private void SaveSettings()
        {
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
                    $"A fatal error has occurred and the program needs to close. You will lose all unsaved changes.\n" +
                    $"\n" +
                    $"{ex.GetType().Name}: {ex.Message}\n" +
                    $"\n" +
                    $"The program log has been saved to {logFile}.\n" +
                    $"Please report this crash to {AuthorContact} and include this log file with your report.",
                    "Unhandled Exception");
            }

            Environment.Exit(1);
        }
    }
}
