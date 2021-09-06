using System;
using Markdig;
using Newtonsoft.Json;

namespace GTA3SaveEditor.Core.Util
{
    public class GitHubReleaseInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tag_name")]
        public string Tag { get; set; }

        [JsonProperty("published_at")]
        public DateTime Date { get; set; }

        [JsonProperty("html_url")]
        public string Url { get; set; }

        [JsonProperty("body")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("prerelease")]
        public bool IsPreRelease { get; set; }

        [JsonProperty("assets")]
        public GitHubAssetInfo[] Assets { get; set; }

        public string ReleaseNotesHtml => Markdown.ToHtml(ReleaseNotes);

        public GitHubReleaseInfo()
        {
            Assets = new GitHubAssetInfo[0];
        }
    }
}
