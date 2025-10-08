using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DSO_Utilities.Updater
{
    public static class UpdateChecker
    {
        public static async Task CheckAsync()
        {
            string owner = "MateHUNAa";
            string repo = "DSO_Utilities";

            var release = await GithubClient.GetLatestReleaseAsync(owner, repo);
            string latestTag = release.tag_name.TrimStart('v', 'V');

            Version current = Assembly.GetExecutingAssembly().GetName().Version;
            if (Version.TryParse(latestTag, out Version latest) && latest > current)
            {
                var result = System.Windows.Forms.MessageBox.Show(
                    $"A new version ({latestTag}) is available.\n\n{release.body}\n\nDownload and update now?",
                    "Update Available",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Information
                );

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    var asset =  release.assets?[0];
                    if (asset == null)
                    {
                        System.Windows.Forms.MessageBox.Show("No downloadable asset found for the latest release.");
                        return;
                    }

                    string zipPath = await DownloadFileAsync(asset.browser_download_url);
                    Updater.ReplaceWith(zipPath);
                }
            }
        }

        private static async Task<string> DownloadFileAsync(string url)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "DSO_Utilities_Update");
            Directory.CreateDirectory(tempDir);

            string dest = Path.Combine(tempDir, "update.zip");

            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = File.Create(dest))
            {
                await stream.CopyToAsync(fileStream);
                return dest;
            }
        }
    }
}
