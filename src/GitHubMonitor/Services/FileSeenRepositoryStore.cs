using GitHubMonitor.Resources;
using System.IO;
using System.Text.Json;

namespace GitHubMonitor.Services;


/// <summary>
/// A repository store that persists seen repository IDs to a JSON file in the local application data folder.   
/// </summary>
public sealed class FileSeenRepositoryStore : ISeenRepositoryStore
{
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    private readonly HashSet<long> _ids;
    private readonly string _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSeenRepositoryStore"/> class. It creates the necessary directory and loads the seen repository IDs 
    /// from the JSON file if it exists; otherwise, it initializes an empty set of IDs.
    /// </summary>
    public FileSeenRepositoryStore()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Paths.SEEN_FILE_DIRECTORY_NAME);
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, Paths.SEEN_STORE_FILENAME);
        _ids = File.Exists(_path)
            ? JsonSerializer.Deserialize<HashSet<long>>(File.ReadAllText(_path), Json) ?? new HashSet<long>()
            : new HashSet<long>();
    }

    /// <summary>
    /// Checks if the specified repository ID has not been seen before. If the ID is not in the set of seen IDs, it is considered new and returns true; otherwise, it returns false.
    /// </summary>
    /// <param name="id">The repository ID to check.</param>
    /// <returns>True if the repository ID has not been seen before; otherwise, false.</returns>
    public bool IsNew(long id) => !_ids.Contains(id);

    /// <summary>
    /// Marks the specified repository ID as seen by adding it to the set of seen IDs.
    /// </summary>
    /// <param name="id">The repository ID to mark as seen.</param>
    public void MarkSeen(long id) => _ids.Add(id);

    /// <summary>
    /// Clears all seen repository IDs from the store and persists the changes to the JSON file.
    /// </summary>
    public void Clear() { _ids.Clear(); Persist(); }

    /// <summary>
    /// Persists the current set of seen repository IDs to the JSON file.
    /// </summary>
    public void Persist() => File.WriteAllText(_path, JsonSerializer.Serialize(_ids, Json));
}