using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EasySave.Core.Models;
using EasySave.Core.Services;


namespace EasySave.Core.Services
{
    public static class JobPriorityCoordinator
    {
        private static readonly List<SaveWork> _activeJobs = new List<SaveWork>();
        private static readonly object _lock = new object();

        public static void RegisterJob(SaveWork job)
        {
            lock (_lock) { _activeJobs.Add(job); }
        }

        public static void UnregisterJob(SaveWork job)
        {
            lock (_lock) { _activeJobs.Remove(job); }
        }

        public static bool HasAnyPrioritaryFile(System.Func<string, bool> isFileDone)
        {
            lock (_lock)
            {
                foreach (var job in _activeJobs)
                {
                    foreach (var file in job.FilesToCopy)
                    {
                        var ext = System.IO.Path.GetExtension(file).ToLower();
                        if (ConfigService.PrioritaryExtensions.Contains(ext) && !isFileDone(file))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
