using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage;

namespace tangerineExtension
{
    public sealed class ConfigAgent
    {
        private static readonly string ConfigFile = "webConfig.json";
        private static readonly string UrlKey = "webURL";

        private static readonly Dictionary<string, string> ServerUrls = new Dictionary<string, string>()
        {
            {"dragon", "https://github.com/Top-tech" },
            {"phoenix", "http://www.titanx.xin//" },
            {"unicorn", "https://blog.csdn.net/weixin_48316521/article/details/106644889" },
            {"localhost", "http://127.0.0.1:4200/" },
        };

        private static readonly string DefaultUrl = ServerUrls["phoenix"];

        public static IAsyncOperation<string> GetHomePageUrlAsync()
        {
            return Task.Run(async () =>
            {
                var result = DefaultUrl;
                try
                {
                    var file = await GetConfigFileAsync(ConfigFile).ConfigureAwait(true);
                    if (file != null)
                    {
                        var content = await FileIO.ReadTextAsync(file);
                        result = GetUrlFromConfig(content);
                    }
                    else
                    {
                        await CreateConfigFileAsync(ConfigFile, DefaultUrl);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception while GetHomePageUrlAsync. Error: {ex.Message}");
                }

                return result;
            }).AsAsyncOperation();
        }

        public static IAsyncOperation<string> GetAndUpdateHomePage(string srv)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(srv))
                {
                    return DefaultUrl;
                }

                var url = DefaultUrl;
                if (ServerUrls.ContainsKey(srv.ToLower()))
                {
                    url = ServerUrls[srv.ToLower()];
                }
                await CreateConfigFileAsync(ConfigFile, url);
                return url;
            }).AsAsyncOperation<string>();
        }

        private static async Task<StorageFile> GetConfigFileAsync(string fileName)
        {
            StorageFile file = null;

            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                file = await localFolder.GetFileAsync(fileName);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception while GetConfigFileAsync. Error: {ex.Message}");
            }

            return file;
        }

        private static string GetUrlFromConfig(string configContent)
        {
            string result = DefaultUrl;

            if (JsonObject.TryParse(configContent, out var config))
            {
                try
                {
                    var url = config[UrlKey].GetString();
                    if (ServerUrls.Values.Any(severUrl => url.Equals(severUrl, StringComparison.OrdinalIgnoreCase)))
                    {
                        result = url;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception while GetUrlFromConfig. Error: {ex.Message}");
                }
            }
            return result;
        }

        private static async Task CreateConfigFileAsync(string fileName, string url)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    var content = $"{{\"{UrlKey}\":\"{url}\"}}";
                    await FileIO.WriteTextAsync(file, content);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception while CreateConfigFileAsync. Error: {ex.Message}");
            }
        }
    }
}
