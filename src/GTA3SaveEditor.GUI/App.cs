using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Game;
using GTA3SaveEditor.Core.Loaders;
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
            Log.Info($"GTA3 Save Editor: {AssemblyFileVersion}");
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
            // TODO: load files from GTA3 dir if known

            LoadGxtTable();
            LoadCarColors();
            LoadScmOpcodes();
            LoadIdeObjects();
        }

        private void LoadGxtTable()
        {
            GTA3.GxtTable = GxtLoader.Load(LoadResource("american.gxt"));
        }

        private void LoadCarColors()
        {
            GTA3.CarColors = CarColorsLoader.LoadColors(LoadResource("carcols.dat"));
        }

        private void LoadIdeObjects()
        {
            byte[] gta3Ide = LoadResource("gta3.ide");
            byte[] defaultIde = LoadResource("default.ide");

            List<IdeObject> objects = new List<IdeObject>();
            objects.Add(new IdeObject(0, ""));
            objects.AddRange(IdeLoader.LoadObjects(gta3Ide));
            objects.AddRange(IdeLoader.LoadObjects(defaultIde));
            GTA3.IdeObjects = objects.OrderBy(x => x.ModelName).ToList();

            List<VehicleModel> cars = new List<VehicleModel>();
            cars.Add(new VehicleModel(0, "", ""));
            cars.AddRange(IdeLoader.LoadCars(defaultIde));
            GTA3.Vehicles = cars.OrderBy(x => x.ToString());
        }

        private void LoadScmOpcodes()
        {
            byte[] scmIni = LoadResource("scm.ini");

            var opDefs = new Dictionary<Opcode, int>();
            var opDefIni = IniLoader.LoadIni(scmIni, "OPCODES");

            foreach (var opDef in opDefIni)
            {
                short op = Convert.ToInt16(opDef.Key, 16);
                string[] def = opDef.Value.Split(',');
                int cnt = int.Parse(def[0]);
                opDefs.Add((Opcode) op, cnt);
            }

            GTA3ScriptParser.OpcodeDefinitions = opDefs;
            Log.Info($"Loaded {opDefs.Count} SCM opcode definitions.");
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
