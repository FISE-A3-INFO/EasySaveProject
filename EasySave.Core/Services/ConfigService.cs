using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EasySave.Core.Services
{
    public static class ConfigService
    {
        public static List<string> PrioritaryExtensions { get; private set; }

        static ConfigService()
        {
            var configPath = "app_config.json";
            if (File.Exists(configPath))
            {
                var configText = File.ReadAllText(configPath);
                var configJson = JsonSerializer.Deserialize<Dictionary<string, object>>(configText);
                if (configJson != null && configJson.ContainsKey("PrioritaryExtensions"))
                {
                    var arr = ((JsonElement)configJson["PrioritaryExtensions"]).EnumerateArray();
                    PrioritaryExtensions = arr.Select(x => x.GetString()?.ToLower() ?? "")
                                             .Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
                else
                {
                    PrioritaryExtensions = new List<string>();
                }
            }
            else
            {
                PrioritaryExtensions = new List<string>();
            }
        }
    }
}
