using GitHubMonitor.Models;

namespace GitHubMonitor.Interfaces;

/// <summary>
/// Interface for a service that displays toast notifications.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Shows a toast notification for a new GitHub repository.
    /// </summary>
    /// <param name="repo">The GitHub repository to display in the toast notification.</param>
    void ShowNewRepo(GitHubRepo repo);

    /// <summary>
    /// Shows a toast notification with the specified title and body.
    /// </summary>
    /// <param name="title">The title of the toast notification.</param>
    /// <param name="body">The body content of the toast notification.</param>
    void ShowInfo(string title, string body);
}
