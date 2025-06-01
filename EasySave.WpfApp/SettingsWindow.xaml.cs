using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using EasySave.WpfApp.Services;
using EasySave.Core.Services; // AJOUT pour PauseService

namespace EasySave.WpfApp
{
    public partial class SettingsWindow : Window
    {
        // Référence rapide à la ressource "Res" dans App.xaml
        private ResourceService _res => (ResourceService)Application.Current.Resources["Res"];

        public SettingsWindow()
        {
            InitializeComponent();

            string currentLang = LoadSavedLanguage() ?? "fr";
            SetComboBoxLanguage(currentLang);

            string currentLogFormat = LoadSavedLogFormat() ?? "json";
            SetComboBoxLogFormat(currentLogFormat);

            string currentSoft = LoadSavedBusinessSoftware() ?? "";
            BusinessSoftwareTextBox.Text = currentSoft;
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var langItem = (ComboBoxItem)LanguageComboBox.SelectedItem;
            string selectedLanguage = langItem?.Tag?.ToString() ?? "fr";

            var logItem = (ComboBoxItem)LogFormatComboBox.SelectedItem;
            string selectedLogFormat = logItem?.Tag?.ToString() ?? "json";

            // Logiciel métier
            string businessSoft = BusinessSoftwareTextBox.Text?.Trim() ?? "";

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

            var config = new
            {
                Language = selectedLanguage,
                LogFormat = selectedLogFormat,
                BusinessSoftware = businessSoft
            };

            string configPath = "app_config.json";
            File.WriteAllText(configPath, JsonSerializer.Serialize(config));

            // **PauseService**
            PauseService.SetTargetProcess(businessSoft);

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

        private string? LoadSavedBusinessSoftware()
        {
            if (!File.Exists("app_config.json"))
                return null;

            try
            {
                var json = File.ReadAllText("app_config.json");
                var config = JsonSerializer.Deserialize<ConfigModel>(json);
                return config?.BusinessSoftware;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        // Classe interne pour désérialisation
        public class ConfigModel
        {
            public string? Language { get; set; }
            public string? LogFormat { get; set; }
            public string? BusinessSoftware { get; set; }
        }
    }
}
