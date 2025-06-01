using System.IO;
using System.Text.Json;
using System.Windows;
using EasySave.Core.Services;

namespace EasySave.WpfApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Charger le process cible si défini en config
            string? configPath = "app_config.json";
            string? targetProcess = null;
            if (File.Exists(configPath))
            {
                try
                {
                    var json = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<ConfigModel>(json);
                    targetProcess = config?.BusinessApp;
                }
                catch { }
            }
            PauseService.StartMonitoring(targetProcess);
        }
    }

    // À mettre ici si pas déjà défini
    public class ConfigModel
    {
        public string? Language { get; set; }
        public string? LogFormat { get; set; }
        public string? BusinessApp { get; set; }
    }
}
