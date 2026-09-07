using GithubMonitor.Resources;
using GitHubMonitor.Interfaces;
using GitHubMonitor.Models;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;


namespace GitHubMonitor.Services;

/// <summary>
/// Provides functionality to show toast notifications on Windows for new GitHub repositories and informational messages.
/// </summary>
public sealed class WindowsToastService : IToastService
{
    
    private static readonly string AppId = General.AppId;

    /// <summary>
    /// Shows a toast notification for a new GitHub repository. The notification includes the repository's full name, description (if available),
    /// programming language, and star count. If the description is too long, it will be truncated to fit within the notification.
    /// </summary>
    /// <param name="repo">The GitHub repository for which to show the toast notification.</param>
    public void ShowNewRepo(GitHubRepo repo)
    {
        var body = string.IsNullOrWhiteSpace(repo.Description)
            ? $"{repo.LanguageText} · {repo.StarsText}"
            : repo.Description!;
        if (body.Length > 180) body = body[..177] + "...";
        Show(repo.FullName, body, repo.HtmlUrl);
    }

    /// <summary>
    /// Shows an informational toast notification with the specified title and body.    
    /// </summary>
    /// <param name="title">The title of the toast notification.</param>
    /// <param name="body">The body content of the toast notification.</param>
    public void ShowInfo(string title, string body) => Show(title, body, null);

    /// <summary>
    /// Shows a toast notification with the specified title, body, and optional URL. If a URL is provided, clicking the notification will launch the URL in the default browser.
    /// </summary>
    /// <param name="title">The title of the toast notification.</param>
    /// <param name="body">The body content of the toast notification.</param>
    /// <param name="url">The URL to launch when the toast notification is clicked. If null, no action is taken.</param>
    private static void Show(string title, string body, string? url)
    {
        var xml = $"""
            <toast launch="{Escape(url ?? "")}" activationType="protocol">
              <visual>
                <binding template="ToastGeneric">
                  <text>{Escape(title)}</text>
                  <text>{Escape(body)}</text>
                </binding>
              </visual>
            </toast>
            """;
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var toast = new ToastNotification(doc) { ExpirationTime = DateTimeOffset.Now.AddHours(6) };
        ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
    }

    /// <summary>
    /// Escapes special characters in the provided string to ensure it is safe for use in XML. This method uses the built-in SecurityElement.Escape method to perform the escaping. 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string Escape(string value) => System.Security.SecurityElement.Escape(value) ?? "";
}