using System;

namespace EasySave.Core.Models
{
    public class SaveStateEntry
    {
        public string Name { get; set; } = string.Empty;
        public DateTime LastActionTime { get; set; } = DateTime.Now;
        public string State { get; set; } = "Inactive";

        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public int Progression { get; set; }

        public string CurrentFileSource { get; set; } = string.Empty;
        public string CurrentFileTarget { get; set; } = string.Empty;
    }
}
