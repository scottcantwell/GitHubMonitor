using GitHubMonitor.Interfaces;
using GitHubMonitor.Models;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace GitHubMonitor.Services;

/// <summary>
/// Service for searching GitHub repositories using the GitHub API.
/// </summary>
public sealed class GitHubSearchService : IGitHubSearchService
{
    /// <summary>
    /// The JSON serializer options used for deserializing GitHub API responses.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Builds a GitHub search query string based on the provided monitor settings.
    /// </summary>
    /// <param name="monitorSettings">The monitor settings to use for building the query.</param>
    /// <returns>A GitHub search query string.</returns>
    public string BuildQuery(MonitorSettings monitorSettings)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(monitorSettings.Keywords)) sb.Append(monitorSettings.Keywords.Trim()).Append(' ');
        var since = DateTimeOffset.UtcNow.AddHours(-Math.Max(1, monitorSettings.CreatedWithinHours));
        sb.Append("created:>").Append(since.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")).Append(' ');
        if (!string.IsNullOrWhiteSpace(monitorSettings.Language)) sb.Append("language:").Append(monitorSettings.Language.Trim()).Append(' ');
        if (!string.IsNullOrWhiteSpace(monitorSettings.Topic)) sb.Append("topic:").Append(monitorSettings.Topic.Trim()).Append(' ');
        if (monitorSettings.MinStars > 0) sb.Append("stars:>=").Append(monitorSettings.MinStars).Append(' ');
        if (!string.IsNullOrWhiteSpace(monitorSettings.User)) sb.Append("user:").Append(monitorSettings.User.Trim()).Append(' ');
        if (!string.IsNullOrWhiteSpace(monitorSettings.Org)) sb.Append("org:").Append(monitorSettings.Org.Trim()).Append(' ');
        if (monitorSettings.ExcludeForks) sb.Append("fork:false ");
        if (monitorSettings.ExcludeArchived) sb.Append("archived:false ");
        if (!string.IsNullOrWhiteSpace(monitorSettings.ExtraQualifiers)) sb.Append(monitorSettings.ExtraQualifiers.Trim());
        return sb.ToString().Trim();
    }

    /// <summary>
    /// Searches GitHub repositories based on the provided monitor settings and returns a list of matching repositories.
    /// </summary>
    /// <param name="settings">The monitor settings to use for the search.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A read-only list of GitHub repositories that match the search criteria.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the GitHub API rate limit is exceeded or authentication fails.</exception>
    public async Task<IReadOnlyList<GitHubRepo>> SearchAsync(MonitorSettings settings, CancellationToken ct)
    {
        using var http = new HttpClient { BaseAddress = new Uri("https://api.GitHub.com/") };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubNewRepoMonitor-WPF/1.0");
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.GitHub+json"));
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        if (!string.IsNullOrWhiteSpace(settings.Token))
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Token);

        var perPage = Math.Clamp(settings.MaxResultsPerPoll, 1, 100);
        var url = $"search/repositories?q={Uri.EscapeDataString(BuildQuery(settings))}&sort=updated&order=desc&per_page={perPage}";
        using var response = await http.GetAsync(url, ct);
        if ((int)response.StatusCode == 403)
            throw new InvalidOperationException(Resources.ErrorMessages.RATE_LIMIT_);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SearchResponse>(JsonOptions, ct)
                      ?? new SearchResponse();
        return payload.Items;
    }
}