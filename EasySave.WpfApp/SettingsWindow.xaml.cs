using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace EasySave.WpfApp
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void SaveSettings_Click(object sender, RoutedEventArgs e)
		{
			string selectedLanguage = ((ComboBoxItem)LanguageComboBox.SelectedItem)?.Tag.ToString();
			string selectedLogFormat = ((ComboBoxItem)LogFormatComboBox.SelectedItem)?.Tag.ToString();

			if (string.IsNullOrEmpty(selectedLanguage) || string.IsNullOrEmpty(selectedLogFormat))
			{
				MessageBox.Show("Veuillez sélectionner une langue et un format de log.");
				return;
			}

			var config = new
			{
				Language = selectedLanguage,
				LogFormat = selectedLogFormat
			};

			string configPath = "app_config.json";
			File.WriteAllText(configPath, JsonSerializer.Serialize(config));

			MessageBox.Show("Paramètres sauvegardés !");
			this.Close();
		}
	}
}
