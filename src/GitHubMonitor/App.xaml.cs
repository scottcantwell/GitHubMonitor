using GitHubMonitor.Interfaces;
using GitHubMonitor.Services;
using GitHubMonitor.ViewModels;
using GitHubMonitor.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GitHubMonitor;

public partial class App : Application
{

    /// <summary>
    /// Gets the service provider for dependency injection. This property is initialized during application startup and provides access to registered services throughout the application.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Handles the startup event of the application. Configures dependency injection, registers services, and initializes the main window with its ViewModel.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();
        sc.AddSingleton<ISettingsStore, FileSettingsStore>();
        sc.AddSingleton<ISeenRepositoryStore, FileSeenRepositoryStore>();
        sc.AddSingleton<IToastService, WindowsToastService>();
        sc.AddSingleton<IGitHubSearchService, GitHubSearchService>();
        sc.AddSingleton<IUrlOpener, ProcessUrlOpener>();
        sc.AddTransient<MainViewModel>();
        sc.AddTransient<MainWindow>();
        Services = sc.BuildServiceProvider();

        var window = Services.GetRequiredService<MainWindow>();
        window.DataContext = Services.GetRequiredService<MainViewModel>();
        window.Show();
    }
}