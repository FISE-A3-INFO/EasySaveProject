using System.IO;
using System.Text.Json;
using EasySave.Logger; // pour accéder à LoggerService
using EasySave.WpfApp; // pour accéder à ConfigModel
using System;
using System.Windows; // <-- INDISPENSABLE pour Application et StartupEventArgs


namespace EasySave.WpfApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erreur critique : {args.ExceptionObject}");
        };

        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erreur WPF : {args.Exception.Message}");
            args.Handled = true;
        };

        // 🔁 Charger le format du log (json/xml) depuis la config
        if (File.Exists("app_config.json"))
        {
            try
            {
                string json = File.ReadAllText("app_config.json");
                var config = JsonSerializer.Deserialize<ConfigModel>(json);
                if (!string.IsNullOrEmpty(config?.LogFormat))
                {
                    LoggerService.LogFormat = config.LogFormat;
                }
            }
            catch
            {
                // Si erreur, on garde "json" par défaut (ne rien faire)
            }
        }

        base.OnStartup(e);
    }
}
