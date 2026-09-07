using System.Text.Json.Serialization;

namespace GitHubMonitor.Models;


/// <summary>
/// Represents a GitHub repository.
/// </summary>
public sealed class GitHubRepo
{


    /// <summary>
    /// Gets or sets the unique identifier of the GitHub repository.
    /// </summary>
    [JsonPropertyName("id")] public long Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the GitHub repository, typically in the format "owner/repo".
    /// </summary>
    [JsonPropertyName("full_name")] public string FullName { get; set; } = "";

    /// <summary>
    /// Gets or sets the HTML URL of the GitHub repository, which can be used to access the repository in a web browser.
    /// </summary>
    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = "";

    /// <summary>
    /// Gets or sets the description of the GitHub repository, providing a brief overview of its purpose or content.
    /// </summary>
    [JsonPropertyName("description")] public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the primary programming language used in the GitHub repository. This property may be null if the language is not specified.
    /// </summary>
    [JsonPropertyName("language")] public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the number of stars (stargazers) for the GitHub repository.
    /// </summary>
    [JsonPropertyName("stargazers_count")] public int Stars { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the GitHub repository, represented as a DateTimeOffset. 
    /// This property indicates when the repository was initially created on GitHub.
    /// </summary>
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the GitHub repository is a fork of another repository.
    /// </summary>
    [JsonPropertyName("fork")] public bool Fork { get; set; }

    /// <summary>
    /// Gets a formatted string representing the number of stars for the GitHub repository, prefixed with a star symbol (★).
    /// </summary>
    public string StarsText => $"★ {Stars}";

    /// <summary>
    /// Gets a formatted string representing the creation date and time of the GitHub repository, converted to the local time zone.
    /// </summary>
    public string CreatedText => CreatedAt.ToLocalTime().ToString("g");

    /// <summary>
    /// Gets a formatted string representing the primary programming language of the GitHub repository. 
    /// If the language is null or whitespace, it returns "n/a".
    /// </summary>
    public string LanguageText => string.IsNullOrWhiteSpace(Language) ? "n/a" : Language;
}