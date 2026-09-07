using System.Text.Json.Serialization;


namespace GitHubMonitor.Models;

/// <summary>
/// Represents the response from the GitHub search API.
/// </summary>
public sealed class SearchResponse
{
    /// <summary>
    /// Gets or sets the total count of repositories matching the search criteria.
    /// </summary>
    [JsonPropertyName("total_count")] public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the list of repositories returned by the search.
    /// </summary>
    [JsonPropertyName("items")] public List<GitHubRepo> Items { get; set; } = new List<GitHubRepo>();
}

