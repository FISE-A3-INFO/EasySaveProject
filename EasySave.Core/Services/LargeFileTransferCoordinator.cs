using System.Threading;

namespace EasySave.Core.Services
{
    public static class LargeFileTransferCoordinator
    {
        private static readonly object _lock = new object();
        private static bool _largeFileInProgress = false;

        public static void EnterIfLargeFile(long fileSizeBytes)
        {
            long threshold = ConfigService.MaxParallelLargeFileSizeKo * 1024L;
            if (fileSizeBytes > threshold)
            {
                lock (_lock)
                {
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] EN ATTENTE gros fichier ({fileSizeBytes} octets > {threshold})...");
                    while (_largeFileInProgress)
                    {
                        Monitor.Wait(_lock);
                    }
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] DEBUT transfert exclusif gros fichier.");
                    _largeFileInProgress = true;
                }
            }
        }
        public static void ExitIfLargeFile(long fileSizeBytes)
        {
            long threshold = ConfigService.MaxParallelLargeFileSizeKo * 1024L;
            if (fileSizeBytes > threshold)
            {
                lock (_lock)
                {
                    _largeFileInProgress = false;
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] FIN transfert exclusif gros fichier.");
                    Monitor.PulseAll(_lock);
                }
            }
        }
    }
}
