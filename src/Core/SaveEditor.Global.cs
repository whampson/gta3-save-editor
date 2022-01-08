using System;
using System.IO;
using System.Security;
using GTA3SaveEditor.Core.Util;
using GTASaveData;
using GTASaveData.GTA3;
using Newtonsoft.Json;

namespace GTA3SaveEditor.Core
{
    public partial class SaveEditor
    {
        public static SaveEditor Instance { get; }
        public static Settings Settings { get; set; }
        
        static SaveEditor()
        {
            Instance = new SaveEditor();
            Settings = Settings.Defaults;
        }

        public static bool LoadSettings(string path)
        {
            string json = File.ReadAllText(path);
            var jsonSettings = new JsonSerializerSettings
            {
                Error = (o, e) =>
                {
                    Log.Exception(e.ErrorContext.Error);
                    e.ErrorContext.Handled = true;
                }
            };

            Settings newSettings = JsonConvert.DeserializeObject<Settings>(json, jsonSettings);
            if (newSettings == null)
            {
                Log.Info("Unable to load settings file.");
                return false;
            }

            Settings = newSettings;
            Log.Info("Loaded settings.");
            return true;
        }

        public static void SaveSettings(string path)
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(path, json);
            Log.Info("Saved settings.");
        }

        public static bool TryLoadFile(string path, out GTA3Save saveFile)
        {
            return TryLoadFile(path, FileType.Default, out saveFile);
        }

        public static bool TryLoadFile(string path, FileType type, out GTA3Save saveFile)
        {
            try
            {
                saveFile = GTA3Save.Load(path, type);
                return saveFile != null;
            }
            catch (Exception e)
            {
                if (e is IOException ||
                    e is SecurityException ||
                    e is UnauthorizedAccessException ||
                    e is SerializationException ||
                    e is InvalidDataException)
                {
                    Log.Error(e);
                    saveFile = null;
                    return false;
                }
                throw;
            }
        }

        public static string DefaultSaveFileDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA3 User Files");
    }

    public enum FileFormatType
    {
        None,
        Android,
        iOS,
        PC,
        PS2,
        PS2AU,
        PS2JP,
        Xbox
    }
}
