namespace GitHubMonitor.Interfaces;

/// <summary>
/// Interface for a service that opens URLs in the default web browser.
/// </summary>
public interface IUrlOpener
{
    /// <summary>
    /// Opens the specified URL in the default web browser.
    /// </summary>
    /// <param name="url"></param>
    void Open(string url);
}
