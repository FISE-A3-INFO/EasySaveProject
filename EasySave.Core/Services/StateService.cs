using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySave.Core.Models;

namespace EasySave.Core.Services
{
    public class StateService
    {
        private static readonly Lazy<StateService> _instance =
            new(() => new StateService());
        public static StateService Instance => _instance.Value;

        private readonly string _stateFilePath;
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        private StateService()
        {
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "EasySaveTasks",
                "state");
            Directory.CreateDirectory(basePath);
            _stateFilePath = Path.Combine(basePath, "state.json");
        }

        public void WriteState(List<SaveStateEntry> states)
        {
            try
            {
                var json = JsonSerializer.Serialize(states, _options);
                File.WriteAllText(_stateFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] State write failed: {ex.Message}");
            }
        }
    }
}
