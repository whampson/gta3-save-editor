using System;
using System.Collections.Generic;
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
        #region Singleton Instance
        public static SaveEditor Instance { get; }
        public static Settings Settings { get; set; }
        public static IEnumerable<CarColor> CarColors { get; set; }
        public static IEnumerable<IdeObject> IdeObjects { get; set; }
        public static IReadOnlyDictionary<string, string> GxtTable { get; set; }

        static SaveEditor()
        {
            Instance = new SaveEditor();
            Settings = Settings.Defaults;
            CarColors = new List<CarColor>();
            IdeObjects = new List<IdeObject>();
            GxtTable = new Dictionary<string, string>();
        }
        #endregion

        #region Settings
        public static bool LoadSettings(string path)
        {
            if (!File.Exists(path))
            {
                Log.Info("Settings file not found.");
                return false;
            }

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
                Log.Error("Unable to load settings file.");
                return false;
            }

            Settings = newSettings;
            return true;
        }

        public static void SaveSettings(string path)
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        #endregion

        #region Static File I/O
        public static bool TryLoadFile(string path, out GTA3Save saveFile)
        {
            return TryLoadFile(path, FileFormat.Default, out saveFile);
        }

        public static bool TryLoadFile(string path, FileFormat format, out GTA3Save saveFile)
        {
            try
            {
                saveFile = SaveData.Load<GTA3Save>(path, format);
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
        #endregion
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
