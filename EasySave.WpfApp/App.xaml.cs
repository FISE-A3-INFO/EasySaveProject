using System.IO;
using System.Text.Json;
using System.Windows;
using EasySave.Core.Services;
using System.Diagnostics;

namespace EasySave.WpfApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            MessageBox.Show("Fermeture de l'application");

            string logFormat = "json";
            string? cryptoSoftPath = null;

            string? configPath = "app_config.json";

            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    CryptoConfig? config = JsonSerializer.Deserialize<CryptoConfig>(json);

                    if (config == null || string.IsNullOrWhiteSpace(config.CryptoSoftPath))
                    {
                        MessageBox.Show("Champ 'CryptoSoftPath' manquant ou vide dans app_config.json");
                        return;
                    }

                    cryptoSoftPath = config.CryptoSoftPath;
                    logFormat = config.LogFormat ?? "json";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lecture config : {ex.Message}");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(cryptoSoftPath))
            {
                MessageBox.Show($"Le chemin de CryptoSoft est invalide.: {cryptoSoftPath}");
                return;
            }

            string logFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "EasySaveTasks", "log", $"{DateTime.Now:yyyy-MM-dd}.{logFormat}");

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = cryptoSoftPath,
                    Arguments = $"\"{logFile}\" maCleUltraSecrete",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    string output = process?.StandardOutput.ReadToEnd() ?? "";
                    string error = process?.StandardError.ReadToEnd() ?? "";
                    process?.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show($"Erreur CryptoSoft : {error}");
                    }
                    else
                    {
                        MessageBox.Show($"CryptoSoft lancé avec succès : {output}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception CryptoSoft : {ex.Message}");
            }
        }
    }

    public class ConfigModel
    {
        public string? Language { get; set; }
        public string? LogFormat { get; set; }
        public string? BusinessApp { get; set; }
        
    }

    public class CryptoConfig
    {
        public string? LogFormat { get; set; }
        public string? CryptoSoftPath { get; set; }
    }
}
