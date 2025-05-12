using System;
using EasySave.Core.Enums;
using EasySave.Core.Models;
using EasySave.Core.Services;
using EasySave.ConsoleApp.Services;
using EasySave.ConsoleApp.Services.Backup;

namespace EasySave.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "EasySave v1.0";
            var manager = SaveManager.Instance;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== EasySave v1.0 ===\n");
                Console.WriteLine("1. Lister les sauvegardes");
                Console.WriteLine("2. Ajouter une sauvegarde");
                Console.WriteLine("3. Exécuter des sauvegardes (1-3 ou 1;3)");
                Console.WriteLine("4. Quitter");
                Console.Write("\nChoix : ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        manager.ListSaveWorks();
                        Pause();
                        break;
                    case "2":
                        Console.Clear();
                        AddSaveWork();
                        Pause();
                        break;
                    case "3":
                        Console.Write("\nCommande (1-3 ou 1;3) : ");
                        var input = Console.ReadLine() ?? string.Empty;
                        var idxs = CommandParser.Parse(input);
                        manager.ExecuteMultiple(idxs.ToArray());
                        Pause();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Pause();
                        break;
                }
            }
        }

        static void AddSaveWork()
        {
            var manager = SaveManager.Instance;
            if (manager.SaveWorks.Count >= 5)
            {
                Console.WriteLine("Limite de 5 sauvegardes atteinte.");
                return;
            }

            Console.Write("Nom de la sauvegarde : ");
            var name = Console.ReadLine() ?? "Unnamed";

            Console.Write("Répertoire source : ");
            var src = Console.ReadLine() ?? "";
            while (!Directory.Exists(src))
            {
                Console.Write("❌ Ce dossier n'existe pas. Réessaie : ");
                src = Console.ReadLine() ?? "";
            }

            Console.Write("Répertoire cible : ");
            var tgt = Console.ReadLine() ?? "";
            if (!Directory.Exists(tgt))
            {
                try
                {
                    Directory.CreateDirectory(tgt);
                    Console.WriteLine("✅ Dossier cible créé.");
                }
                catch
                {
                    Console.WriteLine("❌ Échec création dossier cible.");
                    return;
                }
            }

            Console.Write("Type (1: Complète, 2: Différentielle) : ");
            var t = Console.ReadLine();
            if (t != "1" && t != "2")
            {
                Console.WriteLine("❌ Type invalide.");
                return;
            }

            SaveType type = t == "2"
                ? SaveType.Differential
                : SaveType.Full;

            var work = new SaveWork
            {
                Name = name,
                SourcePath = src,
                TargetPath = tgt,
                Type = type
            };

            if (manager.AddSaveWork(work))
                Console.WriteLine("✅ Sauvegarde ajoutée !");
        }

        static void Pause()
        {
            Console.WriteLine("\nAppuie sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
}
