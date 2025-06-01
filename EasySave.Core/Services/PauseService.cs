using System;
using System.Timers;
using System.Diagnostics;

namespace EasySave.Core.Services
{
    public static class PauseService
    {
        private static bool _globalPauseRequested;
        public static bool GlobalPauseRequested => _globalPauseRequested;

        public static string? TargetProcessName { get; private set; }

        private static System.Timers.Timer? _timer;


        // À appeler au lancement de l'app (App.xaml.cs)
        public static void StartMonitoring(string? processName)
        {
            TargetProcessName = processName;
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            _timer = new System.Timers.Timer(1000); // ou la durée souhaitée
            _timer.Elapsed += (s, e) => CheckProcess();
            _timer.Start();
        }

        private static void CheckProcess()
        {
            if (string.IsNullOrWhiteSpace(TargetProcessName))
            {
                _globalPauseRequested = false;
                return;
            }
            var pname = TargetProcessName.Replace(".exe", "", StringComparison.OrdinalIgnoreCase).Trim();
            var found = Process.GetProcessesByName(pname).Length > 0;
            _globalPauseRequested = found;
        }

        // Pour changer la cible à chaud depuis les paramètres
        public static void SetTargetProcess(string? processName)
        {
            StartMonitoring(processName);
        }

        // Pour pause/reprise manuelle si tu veux un bouton plus tard
        public static void PauseAllJobs() => _globalPauseRequested = true;
        public static void ResumeAllJobs() => _globalPauseRequested = false;
    }
}
