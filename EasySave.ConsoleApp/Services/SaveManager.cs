using System;
using System.Collections.Generic;
using EasySave.Core.Models;
using EasySave.Core.Enums;
using EasySave.Core.Services;
using EasySave.ConsoleApp.Services.Backup;
using EasySave.Logger;

namespace EasySave.ConsoleApp.Services
{
    public class SaveManager
    {
        private static readonly Lazy<SaveManager> _instance =
            new(() => new SaveManager());
        public static SaveManager Instance => _instance.Value;

        private readonly List<SaveWork> _works = new();

        private SaveManager() { }

        public IReadOnlyList<SaveWork> SaveWorks => _works.AsReadOnly();

        public bool AddSaveWork(SaveWork work)
        {
            if (_works.Count >= 5)
            {
                Console.WriteLine("Maximum atteint.");
                return false;
            }
            _works.Add(work);

            // ===== LOG : ajout d'un travail =====
            LoggerService.Instance.Log(new LogEntry
            {
                Name = work.Name,
                FileSource = work.SourcePath,
                FileTarget = work.TargetPath,
                FileSize = 0,
                FileTransferTime = 0,
                Time = DateTime.Now
            });

            return true;
        }

        public void ListSaveWorks()
        {
            Console.WriteLine("=== Save Works ===");
            if (_works.Count == 0)
            {
                Console.WriteLine("Aucune sauvegarde définie.");
                return;
            }
            for (int i = 0; i < _works.Count; i++)
            {
                var w = _works[i];
                Console.WriteLine($"{i + 1}. [{w.Type}] {w.Name} : {w.SourcePath} -> {w.TargetPath}");
            }
        }

        public void ExecuteSaveWork(int index)
        {
            if (index < 0 || index >= _works.Count)
            {
                Console.WriteLine("Index invalide.");
                return;
            }
            var work = _works[index];
            Console.WriteLine($"-> Exécution de {work.Name}...");
            IBackupService svc = work.Type switch
            {
                SaveType.Full => new FullBackupService(),
                SaveType.Differential => new DifferentialBackupService(),
                _ => throw new NotImplementedException()
            };

            try
            {
                svc.Execute(work);
                Console.WriteLine($"✔ {work.Name} terminé.");

                // ===== LOG : exécution réussite =====
                LoggerService.Instance.Log(new LogEntry
                {
                    Name = work.Name,
                    FileSource = work.SourcePath,
                    FileTarget = work.TargetPath,
                    FileSize = 0, // à remplir si possible
                    FileTransferTime = 0, // à remplir si possible
                    Time = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sur {work.Name}: {ex.Message}");

                // ===== LOG : exécution échouée =====
                LoggerService.Instance.Log(new LogEntry
                {
                    Name = work.Name,
                    FileSource = work.SourcePath,
                    FileTarget = work.TargetPath,
                    FileSize = 0,
                    FileTransferTime = 0,
                    Time = DateTime.Now
                });
            }
        }

        public void ExecuteMultiple(params int[] indexes)
        {
            foreach (var i in indexes)
                ExecuteSaveWork(i);
        }
    }
}
