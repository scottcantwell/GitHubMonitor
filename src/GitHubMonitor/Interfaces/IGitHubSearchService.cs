using GitHubMonitor.Models;

namespace GitHubMonitor.Interfaces;

/// <summary>
/// Interface for a service that searches GitHub repositories based on specified settings.
/// </summary>
public interface IGitHubSearchService
{
    /// <summary>
    /// Builds a GitHub search query string based on the provided settings.
    /// </summary>
    /// <param name="settings">The monitor settings to use for building the query.</param>
    /// <returns>A GitHub search query string.</returns>
    string BuildQuery(MonitorSettings settings);

    /// <summary>
    /// Searches GitHub repositories asynchronously based on the provided settings.
    /// </summary>
    /// <param name="settings">The monitor settings to use for the search.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of GitHub repositories.</returns>
    Task<IReadOnlyList<GitHubRepo>> SearchAsync(MonitorSettings settings, CancellationToken ct);
}
