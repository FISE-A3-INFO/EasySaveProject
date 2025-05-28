using System.Windows;
using EasySave.Core.Models;
using System.Collections.ObjectModel;
using EasySave.WpfApp.Services;
using EasySave.Logger; // N'oublie pas ce using !
using System.Linq;

namespace EasySave.WpfApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<SaveWork> _jobs;
        public ObservableCollection<LogEntry> Logs { get; } = new();
        
        public MainWindow()
        {
            InitializeComponent();

            
            _jobs = new ObservableCollection<SaveWork>(SaveManager.Instance.SaveWorks);
            JobsDataGrid.ItemsSource = _jobs;

            LogsDataGrid.ItemsSource = Logs;

            
            foreach (var log in LoggerService.Instance.LoadTodayLogs().Reverse())
                Logs.Add(log);

            LoggerService.Instance.OnNewLog += log =>
            {
                Dispatcher.Invoke(() => Logs.Insert(0, log));
            };
        }

        private void AddJob_Click(object sender, RoutedEventArgs e)
        {
            var addJobWindow = new AddJobWindow
            {
                Owner = this
            };
            if (addJobWindow.ShowDialog() == true && addJobWindow.NewJob != null)
            {
                SaveManager.Instance.AddSaveWork(addJobWindow.NewJob);
                _jobs.Add(addJobWindow.NewJob);
            }
        }
        private async void RunJob_Click(object sender, RoutedEventArgs e)
        {
            if (JobsDataGrid.SelectedItem is SaveWork selectedJob)
            {
                int index = _jobs.IndexOf(selectedJob);
                if (index >= 0)
                {
                    try
                    {
                        await Task.Run(() => SaveManager.Instance.ExecuteSaveWork(index));
                        MessageBox.Show($"Sauvegarde '{selectedJob.Name}' effectuée !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de l'exécution : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde dans la liste.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteJob_Click(object sender, RoutedEventArgs e)
        {
            if (JobsDataGrid.SelectedItem is SaveWork selectedJob)
            {
                var result = MessageBox.Show(
                    $"Confirmer la suppression de '{selectedJob.Name}' ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );
                if (result == MessageBoxResult.Yes)
                {
                    int index = _jobs.IndexOf(selectedJob);
                    if (index >= 0)
                    {
                        _jobs.RemoveAt(index);
                        SaveManager.Instance.RemoveSaveWork(index);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un job à supprimer.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetJobs_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Confirmer la réinitialisation de tous les jobs ?", "Confirmation",
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SaveManager.Instance.ResetJobs();
                _jobs.Clear();
            }
        }
    }
}
