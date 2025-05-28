using System.Windows;
using EasySave.Core.Models;
using EasySave.Core.Services;
using System.Collections.ObjectModel;
using EasySave.WpfApp.Services;

namespace EasySave.WpfApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<SaveWork> _jobs;

        public MainWindow()
        {
            InitializeComponent();
            _jobs = new ObservableCollection<SaveWork>(SaveManager.Instance.SaveWorks);
            JobsDataGrid.ItemsSource = _jobs;
        }

        private void AddJob_Click(object sender, RoutedEventArgs e)
        {
            // Juste pour la démo : ajoute un job fictif
            var job = new SaveWork
            {
                Name = "NouveauJob",
                SourcePath = @"C:\Source",
                TargetPath = @"C:\Target",
                Type = EasySave.Core.Enums.SaveType.Full
            };
            SaveManager.Instance.AddSaveWork(job);
            _jobs.Add(job);
        }
    }
}
