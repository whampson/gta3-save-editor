using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace GTA3SaveEditor.Core.Util
{
    public static class GitHubUpdater
    {
        public static async Task<GitHubReleaseInfo[]> GetReleaseInfoAsync(string repoName, string userAgent = null)
        {
            HttpWebRequest req = WebRequest.CreateHttp($"https://api.github.com/repos/{repoName}/releases");
            req.Method = WebRequestMethods.Http.Get;
            req.Accept = "application/vnd.github.v3+json";
            req.UserAgent = userAgent ?? Assembly.GetExecutingAssembly().GetName().Name;

            using var resp = await HttpHelper.GetHttpResponseAsync(req);
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                return new GitHubReleaseInfo[0];
            }

            using var contentStream = resp.GetResponseStream();
            using StreamReader contentReader = new StreamReader(contentStream);

            if (!JsonHelper.TryParseJson(contentReader.ReadToEnd(), out GitHubReleaseInfo[] releaseInfo))
            {
                Log.Error("Response is not a valid GitHub Release JSON object.");
                return new GitHubReleaseInfo[0];
            }

            return releaseInfo;
        }
    }
}
