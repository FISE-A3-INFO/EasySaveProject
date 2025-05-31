using System.Configuration;
using System.Data;
using System.Windows;

namespace EasySave.WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erreur critique : {args.ExceptionObject}");
        };

        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show($"Erreur WPF : {args.Exception.Message}");
            args.Handled = true;
        };

        base.OnStartup(e);
    }
}

