using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DSO_Utilities.Updater
{

    public class GithubRelease
    {
        public string tag_name { get; set; }
        public string name { get; set; }
        public string body { get; set; }
        public GithubAsset[] assets { get; set; }
    }

    public class GithubAsset
    {
        public string name { get; set; }
        public string browser_download_url { get; set; }
    }


    public static class GithubClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public static async Task<GithubRelease> GetLatestReleaseAsync(string owner, string repo)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DSO-Utilities-Updater");
            string url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            string json = await httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<GithubRelease>(json);
        }
    }
}
