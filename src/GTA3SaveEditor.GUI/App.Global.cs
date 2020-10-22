using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using GTA3SaveEditor.Core;
using GTA3SaveEditor.Core.Util;
using Semver;

namespace GTA3SaveEditor.GUI
{
    public partial class App
    {
        public static string Name => "GTA III Save Editor";
        public static string ShortName => "gta3-save-editor";
        public static string ProjectName => "whampson/gta3-save-editor";
        public static string Copyright => $"(C) 2015-2020 {Author}";
        public static string Author => "Wes Hampson";
        public static string AuthorAlias => "thehambone";
        public static string AuthorContact => "thehambone93@gmail.com";
        public static Uri AuthorContactMailTo => new Uri($"mailto:{AuthorContact}");
        public static Uri AuthorDonateUrl => new Uri("https://ko-fi.com/thehambone");  // TOOD: paypal?
        public static Uri ProjectUrl => new Uri($"https://github.com/{ProjectName}");
        public static Uri ProjectTopicUrl => new Uri("https://gtaforums.com/index.php?showtopic=784598");
        public static string LogText => LogWriter.ToString();
        public static string SettingsPath => "settings.json";
        public static Version AssemblyFileVersion => new Version(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
        public static string VersionString => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public static MainWindow TheWindow { get; private set; }
        public static bool IsStandaloneBuild { get; private set; }
        
        private static readonly StringWriter LogWriter = new StringWriter();

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

        public static async Task<GitHubReleaseInfo> CheckForUpdate()
        {
            Log.Info("Checking for updates...");

            GitHubReleaseInfo[] releaseInfo = await GitHubUpdater.GetReleaseInfoAsync(ProjectName);
            if (releaseInfo.Length == 0)
            {
                Log.Info("No updates available.");
                return null;
            }

            GitHubReleaseInfo latest = releaseInfo[0];
            if (latest.Assets.Length == 0)
            {
                Log.Error("No assets included with release!");
                return null;
            }

            string newVersionString = latest.Tag;
            if (newVersionString.StartsWith("v"))
            {
                newVersionString = newVersionString.Substring(1);
            }

            SemVersion curVersion = SemVersion.Parse(VersionString);
            if (!SemVersion.TryParse(newVersionString, out SemVersion newVersion))
            {
                Log.Error($"Release version '{newVersionString}' is not of the correct format. Please follow Semantic Versioning.");
                return null;
            }

            if (newVersion > curVersion)
            {
                if (latest.IsPreRelease && SaveEditor.Settings.Updater.PreReleaseRing)
                {
                    Log.Info($"Pre-release version {newVersionString} available!");
                    return latest;
                }
                else
                {
                    Log.Info($"Version {newVersionString} available!");
                    return latest;
                }
            }

            Log.Info("No updates available.");
            return null;
        }
    }
}
