using GitHubMonitor.Interfaces;
using System.Diagnostics;

namespace GitHubMonitor.Services;

/// <summary>
/// Opens a URL using the default system browser.
/// </summary>
public sealed class ProcessUrlOpener : IUrlOpener
{
    /// <summary>
    /// Opens the specified URL in the default system browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    public void Open(string url) =>
        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
}