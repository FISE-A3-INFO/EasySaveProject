using System;
using System.IO;
using System.Text.Json;

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
                    // On ouvre en "append" mais XML n'est pas pensé pour concaténer plusieurs objets ! 
                    // Donc chaque entrée sera un <LogEntry>... séparé (ce qui est OK pour la 1.1 mais pas du "vrai" XML array)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Logging failed: {ex.Message}");
            }
        }
    }
}
