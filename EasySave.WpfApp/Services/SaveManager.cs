using System;
using System.Collections.Generic;
using EasySave.Core.Models;
using EasySave.Core.Enums;
using EasySave.Core.Services;
using EasySave.Core.Services.Backup;
using EasySave.Logger;
using System.Text.Json;
using System.IO;

namespace EasySave.WpfApp.Services
{
    public class SaveManager
    {
        private static readonly Lazy<SaveManager> _instance =
            new(() => new SaveManager());
        public static SaveManager Instance => _instance.Value;

        private readonly List<SaveWork> _works = new();

        private SaveManager()
        {
            LoadJobsFromDisk();
        }

        public IReadOnlyList<SaveWork> SaveWorks => _works.AsReadOnly();

        public bool AddSaveWork(SaveWork work)
        {
            if (_works.Count >= 5)
            {
                Console.WriteLine("Maximum atteint.");
                return false;
            }
            _works.Add(work);
            SaveJobsToDisk();

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

            work.Status = EasySave.Core.Enums.SaveState.Active;
            try
            {
                svc.Execute(work);
                work.Status = EasySave.Core.Enums.SaveState.Completed;
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
                work.Status = EasySave.Core.Enums.SaveState.Error;

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
        public void RemoveSaveWork(int index)
        {
            if (index >= 0 && index < _works.Count)
                _works.RemoveAt(index);
        }
        private readonly string jobsFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EasySaveTasks", "EasySaveJobs.json");

        // Sauvegarde la liste des jobs sur disque
        public void SaveJobsToDisk()
        {
            var dir = Path.GetDirectoryName(jobsFilePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(jobsFilePath, JsonSerializer.Serialize(_works));
        }

        // Charge la liste des jobs depuis le disque
        public void LoadJobsFromDisk()
        {
            if (File.Exists(jobsFilePath))
            {
                var jobs = JsonSerializer.Deserialize<List<SaveWork>>(File.ReadAllText(jobsFilePath));
                if (jobs != null)
                {
                    _works.Clear();
                    _works.AddRange(jobs);
                }
            }
        }
        public void ResetJobs()
        {
            _works.Clear();
            if (File.Exists(jobsFilePath))
                File.Delete(jobsFilePath);
        }


    }
}
