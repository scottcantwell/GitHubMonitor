namespace GitHubMonitor.Services;

/// <summary>
/// Represents a store for tracking seen repository IDs.
/// </summary>
public interface ISeenRepositoryStore
{
    /// <summary>
    /// Determines whether the specified repository ID is new (not seen before).
    /// </summary>
    /// <param name="id">The repository ID to check.</param>
    /// <returns><c>true</c> if the repository ID is new; otherwise, <c>false</c>.</returns>
    bool IsNew(long id);
    
    /// <summary>
    /// Marks the specified repository ID as seen.
    /// </summary>
    /// <param name="id">The repository ID to mark as seen.</param>
    void MarkSeen(long id);
    /// <summary>
    /// Clears all seen repository IDs.
    /// </summary>
    void Clear();

    /// <summary>
    /// Persists the current state of seen repository IDs.
    /// </summary>
    void Persist();
}
