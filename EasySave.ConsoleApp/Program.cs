using System;
using EasySave.ConsoleApp.Services;
using EasySave.Core.Models;
using EasySave.Core.Services;


namespace EasySave.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "EasySave v1.0";
            var manager = SaveManager.Instance;

            // === Gestion multi-langue simple (FR par défaut, switchable) ===
            ResourceService.SetLanguage("fr"); // Par défaut FR
            Console.WriteLine("Choisir la langue / Choose language [FR/EN] (laisser vide pour FR) : ");
            var lang = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lang) && lang.Trim().ToLower().StartsWith("e"))
            {
                ResourceService.SetLanguage("en");
            }
            Console.Clear();

            // === Mode ligne de commande (args) ===
            if (args.Length > 0)
            {
                try
                {
                    var idxs = CommandParser.Parse(args[0]);
                    manager.ExecuteMultiple(idxs.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ResourceService.Get("ErrorArgs") + ex.Message);
                }
                return;
            }

            // === Menu principal ===
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== EasySave v1.0 ===\n");
                Console.WriteLine("1. " + ResourceService.Get("ListJobs"));
                Console.WriteLine("2. " + ResourceService.Get("AddJob"));
                Console.WriteLine("3. " + ResourceService.Get("RunJob"));
                Console.WriteLine("4. " + ResourceService.Get("Quit"));
                Console.Write("\n" + ResourceService.Get("PromptChoice"));

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
                        if (manager.SaveWorks.Count >= 5)
                        {
                            Console.WriteLine(ResourceService.Get("LimitReached"));
                            Pause();
                            break;
                        }
                        AddSaveWork();
                        Pause();
                        break;
                    case "3":
                        Console.Write("\n" + ResourceService.Get("PromptRun"));
                        var cmd = Console.ReadLine();
                        try
                        {
                            var idxs = CommandParser.Parse(cmd);
                            manager.ExecuteMultiple(idxs.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ResourceService.Get("ErrorArgs") + ex.Message);
                        }
                        Pause();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine(ResourceService.Get("InvalidChoice"));
                        Pause();
                        break;
                }
            }
        }

        static void Pause()
        {
            Console.WriteLine("\n" + ResourceService.Get("PressEnter"));
            Console.ReadLine();
        }

        static void AddSaveWork()
        {
            // Ici tu gardes ta logique existante, à améliorer selon besoin
            Console.Write(ResourceService.Get("JobName") + " ");
            var name = Console.ReadLine();
            Console.Write(ResourceService.Get("SourcePath") + " ");
            var src = Console.ReadLine();
            Console.Write(ResourceService.Get("TargetPath") + " ");
            var tgt = Console.ReadLine();
            var work = new SaveWork
            {
                Name = name,
                SourcePath = src,
                TargetPath = tgt
                // ajoute les autres propriétés si besoin
            };
            SaveManager.Instance.AddSaveWork(work);
            Console.WriteLine(ResourceService.Get("JobCreated"));
        }
    }
}
