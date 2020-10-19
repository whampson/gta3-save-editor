using System;
using System.IO;
using System.Reflection;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Util;

namespace GTA3SaveEditor.GUI
{
    public partial class App
    {
        private static readonly StringWriter LogWriter = new StringWriter();
        public static MainWindow TheWindow { get; private set; }

        public static string Name => "GTA III Save Editor";
        public static string ShortName => "gta3-save-editor";
        public static string Copyright => $"(C) 2015-2020 {Author}";
        public static string Author => "Wes Hampson";
        public static string AuthorAlias => "thehambone";
        public static string AuthorContact => "thehambone93@gmail.com";
        public static Uri AuthorContactMailTo => new Uri($"mailto:{AuthorContact}");
        public static Uri AuthorDonateUrl => new Uri(@"https://ko-fi.com/thehambone");  // TOOD: paypal?
        public static Uri ProjectUrl => new Uri(@"https://github.com/whampson/gta3-save-editor");
        public static Uri ProjectTopicUrl => new Uri(@"https://gtaforums.com/index.php?showtopic=784598");
        public static string LogText => LogWriter.ToString();
        private static string SettingsPath => "settings.json";

        public static string Version => Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        public static Version AssemblyFileVersion => new Version(
            Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyFileVersionAttribute>()
            .Version);

        public static byte[] LoadResource(string resourceName)
        {
            return LoadResource(new Uri($"pack://application:,,,/Resources/{resourceName}"));
        }

        public static byte[] LoadResource(Uri resourceUri)
        {
            using MemoryStream m = new MemoryStream();
            GetResourceStream(resourceUri).Stream.CopyTo(m);
            return m.ToArray();
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
    }
}
