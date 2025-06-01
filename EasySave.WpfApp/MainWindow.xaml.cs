using System.Windows;
using EasySave.Core.Models;
using System.Collections.ObjectModel;
using EasySave.WpfApp.Services;
using EasySave.Logger; // N'oublie pas ce using !
using System.Linq;
using EasySave.WpfApp;
using System.Windows.Controls;

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
            // Prend TOUS les jobs sélectionnés
            var selectedJobs = JobsDataGrid.SelectedItems.Cast<SaveWork>().ToList();
            if (selectedJobs.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins une sauvegarde.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Si plusieurs jobs sélectionnés, demande le mode d'exécution
            if (selectedJobs.Count > 1)
            {
                var result = MessageBox.Show(
                    "Lancer toutes les sauvegardes en parallèle ? (Non = séquentiel)",
                    "Mode d'exécution",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Cancel) return;

                if (result == MessageBoxResult.Yes)
                {
                    // PARALLÈLE : tous en même temps
                    var tasks = selectedJobs.Select(job =>
                        Task.Run(() => SaveManager.Instance.ExecuteSaveWork(_jobs.IndexOf(job)))
                    );
                    await Task.WhenAll(tasks);
                }
                else
                {
                    // SÉQUENTIEL : un après l'autre
                    foreach (var job in selectedJobs)
                    {
                        await Task.Run(() => SaveManager.Instance.ExecuteSaveWork(_jobs.IndexOf(job)));
                    }
                }
                MessageBox.Show($"Sauvegardes terminées !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Un seul : classique
                int index = _jobs.IndexOf(selectedJobs[0]);
                await Task.Run(() => SaveManager.Instance.ExecuteSaveWork(index));
                MessageBox.Show($"Sauvegarde '{selectedJobs[0].Name}' effectuée !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog(); // ou .Show() si tu ne veux pas bloquer la MainWindow
        }
        private void PauseJob_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SaveWork job)
                job.PauseRequested = true;
        }
        private void ResumeJob_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SaveWork job)
                job.PauseRequested = false;
        }

        


    }
}
