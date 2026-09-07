namespace GitHubMonitor.Models;


/// <summary>
/// Represents the settings for monitoring GitHub repositories. 
/// </summary>
public sealed class MonitorSettings
{
    /// <summary>
    /// Gets or sets the GitHub API token.
    /// </summary>
    public string Token { get; set; } = "";

    /// <summary>
    /// Gets or sets the polling interval in minutes.
    /// </summary>
    public int PollIntervalMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum number of results to retrieve per poll.
    /// </summary>
    public int MaxResultsPerPoll { get; set; } = 30;

    /// <summary>
    /// Gets or sets the keywords to search for.
    /// </summary>
    public string Keywords { get; set; } = "";

    /// <summary>
    /// Gets or sets the programming language to filter by.
    /// </summary>
    public string Language { get; set; } = "";

    /// <summary>
    /// Gets or sets the topic to filter by.
    /// </summary>
    public string Topic { get; set; } = "";

    /// <summary>
    /// Gets or sets the minimum number of stars a repository must have to be included in the results.
    /// </summary>
    public int MinStars { get; set; }

    /// <summary>
    /// Gets or sets the number of hours within which a repository must have been created to be included in the results.
    /// </summary>
    public int CreatedWithinHours { get; set; } = 24;

    /// <summary>
    /// Gets or sets a value indicating whether to exclude forked repositories from the results.
    /// </summary>
    public bool ExcludeForks { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to exclude archived repositories from the results.
    /// </summary>
    public bool ExcludeArchived { get; set; } = true;

    /// <summary>
    /// Gets or sets the GitHub username to filter by.
    /// </summary>
    public string User { get; set; } = "";

    /// <summary>
    /// Gets or sets the GitHub organization to filter by.
    /// </summary>
    public string Org { get; set; } = "";

    /// <summary>
    /// Gets or sets any extra qualifiers to include in the search query.
    /// </summary>
    public string ExtraQualifiers { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether to notify on start seed.
    /// </summary>
    public bool NotifyOnStartSeed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the criteria panel is visible.
    /// </summary>
    public bool IsCriteriaVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the results panel is visible.
    /// </summary>
    public bool IsResultsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the log panel is visible.
    /// </summary>
    public bool IsLogVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the width of the criteria panel.
    /// </summary>
    public double CriteriaWidth { get; set; } = 340;

    /// <summary>
    /// Gets or sets the height of the log panel.
    /// </summary>
    public double LogHeight { get; set; } = 140;
}