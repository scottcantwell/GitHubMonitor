using GitHubMonitor.Models;

namespace GitHubMonitor.Interfaces;

/// <summary>
/// Interface for a service that loads and saves monitor settings.
/// </summary>
public interface ISettingsStore
{
    /// <summary>
    /// Loads the monitor settings.
    /// </summary>
    /// <returns>The loaded monitor settings.</returns>
    MonitorSettings Load();

    /// <summary>
    /// Saves the specified monitor settings.
    /// </summary>
    /// <param name="settings">The monitor settings to save.</param>
    void Save(MonitorSettings settings);
}
