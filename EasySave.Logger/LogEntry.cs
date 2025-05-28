using System;

namespace EasySave.Logger
{
    public class LogEntry
    {
        public string Name { get; set; } = string.Empty;
        public string FileSource { get; set; } = string.Empty;
        public string FileTarget { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
