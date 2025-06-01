using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EasySave.Core.Services
{
    public static class ConfigService
    {
        public static List<string> PrioritaryExtensions { get; private set; } = new List<string>();
        public static string BusinessSoftware { get; private set; } = "";
        public static int MaxParallelLargeFileSizeKo { get; private set; } = 50000;

        private static string configPath = "app_config.json";

        static ConfigService()
        {
            Load();
        }

        public static void Reload()
        {
            Load();
        }

        private static void Load()
        {
            if (File.Exists(configPath))
            {
                var configText = File.ReadAllText(configPath);
                var configJson = JsonSerializer.Deserialize<Dictionary<string, object>>(configText);
                if (configJson != null)
                {
                    // PrioritaryExtensions
                    if (configJson.ContainsKey("PrioritaryExtensions"))
                    {
                        var arr = ((JsonElement)configJson["PrioritaryExtensions"]).EnumerateArray();
                        PrioritaryExtensions = arr.Select(x => x.GetString()?.ToLower() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToList();
                    }
                    else
                    {
                        PrioritaryExtensions = new List<string>();
                    }

                    // BusinessSoftware
                    if (configJson.ContainsKey("BusinessSoftware"))
                        BusinessSoftware = ((JsonElement)configJson["BusinessSoftware"]).GetString() ?? "";
                    else
                        BusinessSoftware = "";

                    // MaxParallelLargeFileSizeKo
                    if (configJson.ContainsKey("MaxParallelLargeFileSizeKo"))
                        MaxParallelLargeFileSizeKo = ((JsonElement)configJson["MaxParallelLargeFileSizeKo"]).GetInt32();
                    else
                        MaxParallelLargeFileSizeKo = 50000;
                }
                else
                {
                    PrioritaryExtensions = new List<string>();
                    BusinessSoftware = "";
                    MaxParallelLargeFileSizeKo = 50000;
                }
            }
            else
            {
                PrioritaryExtensions = new List<string>();
                BusinessSoftware = "";
                MaxParallelLargeFileSizeKo = 50000;
            }
        }
    }
}
