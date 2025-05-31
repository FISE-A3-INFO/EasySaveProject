using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using EasySave.WpfApp.Services; // <-- on importe ResourceService

namespace EasySave.WpfApp
{
    public partial class SettingsWindow : Window
    {
        // Référence rapide à la ressource "Res" dans App.xaml
        private ResourceService _res => (ResourceService)Application.Current.Resources["Res"];

        public SettingsWindow()
        {
            InitializeComponent();

            // Initialise les ComboBox avec la langue actuellement sélectionnée
            // par défaut "fr", ou ce qui est enregistré dans ton fichier de config.
            string currentLang = LoadSavedLanguage() ?? "fr";
            SetComboBoxLanguage(currentLang);

            // Initialise le format de log (par défaut "json" ou depuis config)
            string currentLogFormat = LoadSavedLogFormat() ?? "json";
            SetComboBoxLogFormat(currentLogFormat);
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Récupère la langue choisie
            var langItem = (ComboBoxItem)LanguageComboBox.SelectedItem;
            string selectedLanguage = langItem?.Tag?.ToString() ?? "fr";

            // Récupère le format de log choisi
            var logItem = (ComboBoxItem)LogFormatComboBox.SelectedItem;
            string selectedLogFormat = logItem?.Tag?.ToString() ?? "json";

            // Si aucun choix valide
            if (string.IsNullOrEmpty(selectedLanguage) || string.IsNullOrEmpty(selectedLogFormat))
            {
                MessageBox.Show(
                    _res["MissingFields"],
                    _res["Error"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Sauvegarde en JSON (ou XML si tu préfères)
            var config = new
            {
                Language = selectedLanguage,
                LogFormat = selectedLogFormat
            };

            string configPath = "app_config.json";
            File.WriteAllText(configPath, JsonSerializer.Serialize(config));

            // Applique immédiatement la nouvelle langue
            _res.SetLanguage(selectedLanguage);

            MessageBox.Show(
                _res["SaveSuccessMessage"],
                _res["SettingsWindowTitle"],
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            this.Close();
        }

        #region Helpers pour initialiser les ComboBox au démarrage

        private void SetComboBoxLanguage(string lang)
        {
            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if (item.Tag?.ToString() == lang)
                {
                    LanguageComboBox.SelectedItem = item;
                    return;
                }
            }
            // si non trouvé, on laisse sur premier
            LanguageComboBox.SelectedIndex = 0;
        }

        private void SetComboBoxLogFormat(string format)
        {
            foreach (ComboBoxItem item in LogFormatComboBox.Items)
            {
                if (item.Tag?.ToString() == format)
                {
                    LogFormatComboBox.SelectedItem = item;
                    return;
                }
            }
            // si non trouvé, on laisse sur premier
            LogFormatComboBox.SelectedIndex = 0;
        }

        private string? LoadSavedLanguage()
        {
            if (!File.Exists("app_config.json"))
                return null;

            try
            {
                var json = File.ReadAllText("app_config.json");
                var config = JsonSerializer.Deserialize<ConfigModel>(json);
                return config?.Language;
            }
            catch
            {
                return null;
            }
        }

        private string? LoadSavedLogFormat()
        {
            if (!File.Exists("app_config.json"))
                return null;

            try
            {
                var json = File.ReadAllText("app_config.json");
                var config = JsonSerializer.Deserialize<ConfigModel>(json);
                return config?.LogFormat;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }

    // Classe interne pour désérialisation
    public class ConfigModel
    {
        public string? Language { get; set; }
        public string? LogFormat { get; set; }
    }
}
