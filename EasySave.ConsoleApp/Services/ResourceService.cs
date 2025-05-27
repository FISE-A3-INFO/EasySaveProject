using System.Collections.Generic;

namespace EasySave.ConsoleApp.Services
{
    public static class ResourceService
    {
        private static string language = "fr";
        private static readonly Dictionary<string, Dictionary<string, string>> translations = new()
        {
            ["fr"] = new Dictionary<string, string>
            {
                ["ListJobs"] = "Lister les sauvegardes",
                ["AddJob"] = "Ajouter une sauvegarde",
                ["RunJob"] = "Exécuter des sauvegardes (1-3 ou 1;3)",
                ["Quit"] = "Quitter",
                ["PromptChoice"] = "Choix : ",
                ["LimitReached"] = "Vous ne pouvez pas avoir plus de 5 travaux de sauvegarde.",
                ["InvalidChoice"] = "Choix invalide. Essayez encore.",
                ["PressEnter"] = "Appuyez sur Entrée pour continuer...",
                ["JobName"] = "Nom du travail :",
                ["SourcePath"] = "Chemin source :",
                ["TargetPath"] = "Chemin cible :",
                ["JobCreated"] = "Travail de sauvegarde créé avec succès.",
                ["PromptRun"] = "Commande (1-3 ou 1;3) : ",
                ["ErrorArgs"] = "Erreur dans les arguments : ",
            },
            ["en"] = new Dictionary<string, string>
            {
                ["ListJobs"] = "List backups",
                ["AddJob"] = "Add a backup",
                ["RunJob"] = "Run backups (1-3 or 1;3)",
                ["Quit"] = "Quit",
                ["PromptChoice"] = "Choice: ",
                ["LimitReached"] = "You can't have more than 5 backup jobs.",
                ["InvalidChoice"] = "Invalid choice. Try again.",
                ["PressEnter"] = "Press Enter to continue...",
                ["JobName"] = "Job name:",
                ["SourcePath"] = "Source path:",
                ["TargetPath"] = "Target path:",
                ["JobCreated"] = "Backup job successfully created.",
                ["PromptRun"] = "Command (1-3 or 1;3): ",
                ["ErrorArgs"] = "Argument error: ",
            }
        };

        public static void SetLanguage(string lang)
        {
            language = translations.ContainsKey(lang) ? lang : "fr";
        }

        public static string Get(string key)
        {
            if (translations[language].ContainsKey(key))
                return translations[language][key];
            // fallback français
            return translations["fr"].ContainsKey(key) ? translations["fr"][key] : key;
        }
    }
}
