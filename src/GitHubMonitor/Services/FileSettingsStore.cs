using GitHubMonitor.Resources;
using GitHubMonitor.Interfaces;
using GitHubMonitor.Models;

using System.IO;
using System.Text.Json;

namespace GitHubMonitor.Services;

/// <summary>
/// A service that loads and saves monitor settings to a JSON file in the local application data folder.
/// </summary>
public sealed class FileSettingsStore : ISettingsStore
{
    /// <summary>
    /// The JSON serializer options used for reading and writing the settings file, with indentation enabled for readability.
    /// </summary>
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

    /// <summary>
    /// Gets the directory path for storing the settings file, which is located in the local application data folder under "GitHubMonitor".
    /// </summary>
    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        Paths.SETTINGS_STORE_FILE_DIRECTORY_NAME);

    /// <summary>
    /// Gets the full file path for the settings file, which is named "settings.json" and located in the directory specified by <see cref="Dir"/>.
    /// </summary>
    private static string PathFile => Path.Combine(Dir, Paths.SETTINGS_FILENAME);

    /// <summary>
    /// Loads the monitor settings from the JSON file. If the file does not exist, a new instance of <see cref="MonitorSettings"/> is returned.
    /// </summary>
    /// <returns></returns>
    public MonitorSettings Load()
    {
        Directory.CreateDirectory(Dir);
        if (!File.Exists(PathFile)) return new MonitorSettings();
        return JsonSerializer.Deserialize<MonitorSettings>(File.ReadAllText(PathFile), Json)
               ?? new MonitorSettings();
    }

    /// <summary>
    /// Saves the specified monitor settings to the JSON file. If the directory does not exist, it is created. The settings are serialized with indentation for readability.    
    /// </summary>
    /// <param name="settings"></param>
    public void Save(MonitorSettings settings)
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(PathFile, JsonSerializer.Serialize(settings, Json));
    }
}