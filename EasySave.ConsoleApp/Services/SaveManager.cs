using System;
using System.Collections.Generic;
using EasySave.Core.Models;
using EasySave.Core.Enums;
using EasySave.Core.Services;
using EasySave.ConsoleApp.Services.Backup;

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
            svc.Execute(work);
            Console.WriteLine($"✔ {work.Name} terminé.");
        }

        public void ExecuteMultiple(params int[] indexes)
        {
            foreach (var i in indexes)
                ExecuteSaveWork(i);
        }
    }
}
