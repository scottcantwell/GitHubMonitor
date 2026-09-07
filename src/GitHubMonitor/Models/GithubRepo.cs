using System.Text.Json.Serialization;

namespace GitHubMonitor.Models;


/// <summary>
/// Represents a GitHub repository.
/// </summary>
public sealed class GitHubRepo
{


    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("full_name")] public string FullName { get; set; } = "";
    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = "";
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("language")] public string? Language { get; set; }
    [JsonPropertyName("stargazers_count")] public int Stars { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("fork")] public bool Fork { get; set; }

    public string StarsText => $"★ {Stars}";
    public string CreatedText => CreatedAt.ToLocalTime().ToString("g");
    public string LanguageText => string.IsNullOrWhiteSpace(Language) ? "n/a" : Language;
}