using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasySave.WpfApp.Services
{
	public class ResourceService : INotifyPropertyChanged
	{
		private string _language = "fr";

		private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
		{
			["fr"] = new Dictionary<string, string>
			{
				["AddButton"] = "Ajouter",
				["AddJobButton"] = "Ajouter un job",
				["CancelButton"] = "Annuler",
				["DeleteJobButton"] = "Supprimer le job sélectionné",
				["JobName"] = "Nom",
				["JobStatus"] = "Statut",
				["JobType"] = "Type",
				["LanguageEnglish"] = "Anglais",
				["LanguageFrench"] = "Français",
				["LanguageLabel"] = "Langue",
				["LogFormatLabel"] = "Format du log",
				["MainWindowTitle"] = "EasySave v3.0",
				["NameLabel"] = "Nom",
				["ResetJobsButton"] = "Réinitialiser la liste de jobs",
				["RunJobButton"] = "Lancer la sauvegarde sélectionnée",
				["SaveButton"] = "Sauvegarder",
				["SettingsButton"] = "Paramètres",
				["SettingsWindowTitle"] = "Paramètres",
				["SourceLabel"] = "Source",
				["SourcePath"] = "Source",
				["TargetLabel"] = "Cible",
				["TargetPath"] = "Cible",
				["TypeDiff"] = "Différentielle",
				["TypeFull"] = "Complète",
				["TypeLabel"] = "Type",
				["WindowTitle"] = "Ajouter une sauvegarde",
				["MissingFields"] = "Veuillez sélectionner une langue et un format de log.",
				["Error"] = "Erreur",
				["SaveSuccessMessage"] = "Paramètres sauvegardés !",

			},
			["en"] = new Dictionary<string, string>
			{
				["AddButton"] = "Add",
				["AddJobButton"] = "Add a job",
				["CancelButton"] = "Cancel",
				["DeleteJobButton"] = "Delete selected job",
				["JobName"] = "Name",
				["JobStatus"] = "Status",
				["JobType"] = "Type",
				["LanguageEnglish"] = "English",
				["LanguageFrench"] = "French",
				["LanguageLabel"] = "Language",
				["LogFormatLabel"] = "Log format",
				["MainWindowTitle"] = "EasySave v3.0",
				["NameLabel"] = "Name",
				["ResetJobsButton"] = "Reset job list",
				["RunJobButton"] = "Run selected backup",
				["SaveButton"] = "Save",
				["SettingsButton"] = "Settings",
				["SettingsWindowTitle"] = "Settings",
				["SourceLabel"] = "Source",
				["SourcePath"] = "Source",
				["TargetLabel"] = "Target",
				["TargetPath"] = "Target",
				["TypeDiff"] = "Differential",
				["TypeFull"] = "Full",
				["TypeLabel"] = "Type",
				["WindowTitle"] = "Add a backup",
				["MissingFields"] = "Please select a language and a log format.",
				["Error"] = "Error",
				["SaveSuccessMessage"] = "Settings saved!",
			}
		};

		public string this[string key]
		{
			get
			{
				if (_translations[_language].ContainsKey(key))
					return _translations[_language][key];
				if (_translations["fr"].ContainsKey(key))
					return _translations["fr"][key]; // fallback
				return key;
			}
		}

		public void SetLanguage(string lang)
		{
			if (_translations.ContainsKey(lang))
			{
				_language = lang;
				OnPropertyChanged(string.Empty); // Notifie que toutes les clés ont potentiellement changé
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string name) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
