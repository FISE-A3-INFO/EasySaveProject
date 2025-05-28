using System.Windows;
using EasySave.Core.Models;
using EasySave.Core.Enums;

namespace EasySave.WpfApp
{
    public partial class AddJobWindow : Window
    {
        public SaveWork? NewJob { get; private set; }

        public AddJobWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Validation ultra simple
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(SourceBox.Text) ||
                string.IsNullOrWhiteSpace(TargetBox.Text) ||
                TypeBox.SelectedItem == null)
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedType = ((System.Windows.Controls.ComboBoxItem)TypeBox.SelectedItem).Tag?.ToString();
            SaveType type = selectedType == "Differential" ? SaveType.Differential : SaveType.Full;

            NewJob = new SaveWork
            {
                Name = NameBox.Text,
                SourcePath = SourceBox.Text,
                TargetPath = TargetBox.Text,
                Type = type
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
