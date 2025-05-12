using EasySave.Core.Enums;

namespace EasySave.Core.Models
{
    public class SaveWork
    {
        public string Name { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public SaveType Type { get; set; } = SaveType.Full;
        public SaveState Status { get; set; } = SaveState.Inactive;
    }
}
