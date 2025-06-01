using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

namespace EasySave.Logger
{
    public class LoggerService
    {
        private static readonly Lazy<LoggerService> _instance =
            new(() => new LoggerService());
        public static LoggerService Instance => _instance.Value;

        private readonly string _logDir;
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        // Permet de choisir le format du log (json ou xml)
        public static string LogFormat { get; set; } = "json";

        // Event pour la notification temps réel
        public event Action<LogEntry>? OnNewLog;

        private LoggerService()
        {
            _logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "EasySaveTasks",
                "log");
            Directory.CreateDirectory(_logDir);
        }

        public void Log(LogEntry entry)
        {
            var fileName = $"{DateTime.Now:yyyy-MM-dd}.{LogFormat}";
            var filePath = Path.Combine(_logDir, fileName);

            try
            {
                if (LogFormat == "xml")
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LogEntry));
                    using (var writer = new StreamWriter(filePath, true))
                    {
                        serializer.Serialize(writer, entry);
                    }
                }
                else // json
                {
                    var json = JsonSerializer.Serialize(entry, _options);
                    File.AppendAllText(filePath, json + Environment.NewLine);
                }

                // Notifier la GUI (ou autres listeners)
                OnNewLog?.Invoke(entry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Logging failed: {ex.Message}");
            }
            

        }

        // Récupère les logs du jour (fichier courant)
        public IEnumerable<LogEntry> LoadTodayLogs()
        {
            var fileName = $"{DateTime.Now:yyyy-MM-dd}.{LogFormat}";
            var filePath = Path.Combine(_logDir, fileName);

            if (!File.Exists(filePath))
                return new List<LogEntry>();

            var logs = new List<LogEntry>();
            if (LogFormat == "xml")
            {
                // Parsing "vraie" liste XML à améliorer si tu veux charger tout le XML
                // Ici, version simplifiée : charge toutes les lignes une par une
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(LogEntry));
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            var entry = (LogEntry?)serializer.Deserialize(reader);
                            if (entry != null) logs.Add(entry);
                        }
                        catch { break; }
                    }
                }
            }
            else // json
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    try
                    {
                        var entry = JsonSerializer.Deserialize<LogEntry>(line);
                        if (entry != null) logs.Add(entry);
                    }
                    catch { }
                }
            }
            return logs;
        }
    }
}
