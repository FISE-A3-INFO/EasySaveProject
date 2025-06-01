using System.ComponentModel;
using EasySave.Core.Enums;

namespace EasySave.Core.Models
{
    public class SaveWork : INotifyPropertyChanged
    {
        private SaveState _status = SaveState.Inactive; // Valeur par défaut

        public SaveState Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public string Name { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public SaveType Type { get; set; } = SaveType.Full;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        private bool _pauseRequested;
        public bool PauseRequested
        {
            get => _pauseRequested;
            set
            {
                if (_pauseRequested != value)
                {
                    _pauseRequested = value;
                    OnPropertyChanged(nameof(PauseRequested));
                }
            }
        }

    }
}
