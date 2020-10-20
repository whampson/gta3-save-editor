using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA3SaveEditor.Core.Extensions;
using GTA3SaveEditor.Core.Util;

namespace GTA3SaveEditor.Core.Helpers
{
    public static class HttpHelper
    {
        public static async Task<HttpWebResponse> GetHttpResponseAsync(HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse) await req.GetResponseAsync();
            }
            catch (WebException e)
            {
                var resp = e.Response as HttpWebResponse;
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    Log.Error($"HTTP {req.Method} returned {(int) resp.StatusCode} ({resp.StatusDescription}).");
                }

                return resp;
            }
        }

        public static async Task DownloadFileAsync(string url, string dest,
            CancellationToken cancellationToken = default,
            IProgress<double> progress = null,
            int timeoutMillis = 300000)
        {
            using HttpClient client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeoutMillis) };
            using FileStream file = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
            await client.DownloadAsync(url, file, cancellationToken, progress);
        }
    }
}
