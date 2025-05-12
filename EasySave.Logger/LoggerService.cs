using System;
using System.IO;
using System.Text.Json;
using EasySave.Core.Models;

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
            var fileName = $"{DateTime.Now:yyyy-MM-dd}.json";
            var filePath = Path.Combine(_logDir, fileName);

            try
            {
                var json = JsonSerializer.Serialize(entry, _options);
                File.AppendAllText(filePath, json + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Logging failed: {ex.Message}");
            }
        }
    }
}
