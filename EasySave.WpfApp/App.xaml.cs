using System.IO;
using System.Text.Json;
using System.Windows;
using EasySave.Core.Services;
using System.Diagnostics;
using EasySave.Logger;

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
                    if (!string.IsNullOrWhiteSpace(config?.LogFormat))
                    {
                        LoggerService.LogFormat = config.LogFormat.ToLower(); // "json" ou "xml"
                    }
                }
                catch { }
            }

            PauseService.StartMonitoring(targetProcess);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            MessageBox.Show("Fermeture de l'application");

            string? cryptoSoftPath = "CryptoSoft\\dist\\CryptoSoftCLI.exe";

    

            if (string.IsNullOrWhiteSpace(cryptoSoftPath) || !File.Exists(cryptoSoftPath))
            {
                MessageBox.Show($"Le chemin de CryptoSoft est invalide ou inexistant : {cryptoSoftPath}");
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cryptoSoftPath,
                    WorkingDirectory = Path.GetDirectoryName(cryptoSoftPath) ?? "",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = psi })
                {
                    process.Start();

                    // Envoie automatique des réponses si besoin
                    using (StreamWriter sw = process.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            sw.WriteLine("n"); // ne pas ouvrir le configurateur
                            sw.WriteLine("e"); // encoder
                            // Ajoute une ligne vide si ton programme demande "Appuyez sur Entrée pour sortir..."
                            sw.WriteLine("");
                        }
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show($"Erreur CryptoSoft : {error}");
                    }
                    else
                    {
                        MessageBox.Show($"CryptoSoft exécuté avec succès.\n\n{output}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception lors de l'exécution de CryptoSoft : {ex.Message}");
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
