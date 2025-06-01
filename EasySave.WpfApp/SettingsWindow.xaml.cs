using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using EasySave.Core.Services;
using EasySave.WpfApp.Services;

namespace EasySave.WpfApp
{
    public partial class SettingsWindow : Window
    {
        // Référence à la ressource "Res"
        private ResourceService _res => (ResourceService)Application.Current.Resources["Res"];

        public SettingsWindow()
        {
            InitializeComponent();

            // --- INIT ---
            string currentLang = LoadSavedLanguage() ?? "fr";
            SetComboBoxLanguage(currentLang);

            string currentLogFormat = LoadSavedLogFormat() ?? "json";
            SetComboBoxLogFormat(currentLogFormat);

            string currentSoft = LoadSavedBusinessSoftware() ?? "";
            BusinessSoftwareTextBox.Text = currentSoft;

            // NEW: Extensions prioritaires
            var currentExtensions = LoadSavedExtensions() ?? new List<string>();
            ExtensionsListBox.ItemsSource = new List<string>(currentExtensions);
        }

        private void AddExtension_Click(object sender, RoutedEventArgs e)
        {
            string ext = AddExtensionTextBox.Text.Trim().ToLower();
            if (!ext.StartsWith(".") && ext != "") ext = "." + ext;
            if (!string.IsNullOrEmpty(ext))
            {
                var list = ExtensionsListBox.Items.Cast<string>().ToList();
                if (!list.Contains(ext))
                {
                    list.Add(ext);
                    ExtensionsListBox.ItemsSource = null;
                    ExtensionsListBox.ItemsSource = list;
                }
            }
            AddExtensionTextBox.Text = "";
        }

        private void RemoveExtension_Click(object sender, RoutedEventArgs e)
        {
            if (ExtensionsListBox.SelectedItem is string ext)
            {
                var list = ExtensionsListBox.Items.Cast<string>().ToList();
                list.Remove(ext);
                ExtensionsListBox.ItemsSource = null;
                ExtensionsListBox.ItemsSource = list;
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var langItem = (ComboBoxItem)LanguageComboBox.SelectedItem;
            string selectedLanguage = langItem?.Tag?.ToString() ?? "fr";

            var logItem = (ComboBoxItem)LogFormatComboBox.SelectedItem;
            string selectedLogFormat = logItem?.Tag?.ToString() ?? "json";

            // Logiciel métier
            string businessSoft = BusinessSoftwareTextBox.Text?.Trim() ?? "";

            // Extensions prioritaires
            var extensions = ExtensionsListBox.Items.Cast<string>().Select(x => x.Trim().ToLower()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

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
                BusinessSoftware = businessSoft,
                PrioritaryExtensions = extensions
            };

            string configPath = "app_config.json";
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

            // Reload dynamique pour prise en compte immédiate côté Core
            ConfigService.Reload();

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

        private List<string>? LoadSavedExtensions()
        {
            if (!File.Exists("app_config.json"))
                return null;

            try
            {
                var json = File.ReadAllText("app_config.json");
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("PrioritaryExtensions", out var arr))
                {
                    return arr.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
                return null;
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
            public List<string>? PrioritaryExtensions { get; set; }
        }
    }
}
