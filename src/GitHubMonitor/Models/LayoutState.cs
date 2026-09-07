namespace GitHubMonitor.ViewModels;

/// <summary>
/// Represents the layout state of the application, including the width of the criteria panel and the height of the log panel.
/// </summary>
/// <param name="CriteriaWidth">The width of the criteria panel.</param>
/// <param name="LogHeight">The height of the log panel.</param>
public sealed record LayoutState(double? CriteriaWidth, double? LogHeight);

