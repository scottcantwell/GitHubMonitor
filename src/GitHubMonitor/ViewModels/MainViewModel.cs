using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitHubMonitor.Interfaces;
using GitHubMonitor.Models;
using GitHubMonitor.Services;
using System.Collections.ObjectModel;

namespace GitHubMonitor.ViewModels;

/// <summary>
/// ViewModel for the main window of the GitHub monitor application.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IGitHubSearchService _search;
    private readonly ISettingsStore _settingsStore;
    private readonly ISeenRepositoryStore _seen;
    private readonly IToastService _toasts;
    private readonly IUrlOpener _urls;
    private CancellationTokenSource? _cts;

    public ObservableCollection<GitHubRepo> Repositories { get; } = new();
    public ObservableCollection<string> LogLines { get; } = new();

    [ObservableProperty] private string token = "";
    [ObservableProperty] private string keywords = "";
    [ObservableProperty] private string language = "";
    [ObservableProperty] private string topic = "";
    [ObservableProperty] private string user = "";
    [ObservableProperty] private string org = "";
    [ObservableProperty] private string extraQualifiers = "";
    [ObservableProperty] private int minStars;
    [ObservableProperty] private int createdWithinHours = 24;
    [ObservableProperty] private int pollIntervalMinutes = 5;
    [ObservableProperty] private int maxResultsPerPoll = 30;
    [ObservableProperty] private bool excludeForks = true;
    [ObservableProperty] private bool excludeArchived = true;
    [ObservableProperty] private bool notifyOnStartSeed;
    [ObservableProperty] private bool isRunning;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusText = "Idle.";
    [ObservableProperty] private string queryPreview = "";
    [ObservableProperty] private GitHubRepo? selectedRepository;
    [ObservableProperty] private bool isCriteriaVisible = true;
    [ObservableProperty] private bool isResultsVisible = true;
    [ObservableProperty] private bool isLogVisible = true;
    [ObservableProperty] private double criteriaWidth = 340;
    [ObservableProperty] private double logHeight = 140;

    public string StartStopLabel => IsRunning ? "Stop monitoring" : "Start monitoring";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class with the specified services.
    /// </summary>
    /// <param name="search"></param>
    /// <param name="settingsStore"></param>
    /// <param name="seen"></param>
    /// <param name="toasts"></param>
    /// <param name="urls"></param>
    public MainViewModel(
        IGitHubSearchService search,
        ISettingsStore settingsStore,
        ISeenRepositoryStore seen,
        IToastService toasts,
        IUrlOpener urls)
    {
        _search = search;
        _settingsStore = settingsStore;
        _seen = seen;
        _toasts = toasts;
        _urls = urls;
        LoadFrom(_settingsStore.Load());
        RefreshQueryPreview();
        Log("Ready.");
    }

    partial void OnKeywordsChanged(string value) => RefreshQueryPreview();
    partial void OnLanguageChanged(string value) => RefreshQueryPreview();
    partial void OnTopicChanged(string value) => RefreshQueryPreview();
    partial void OnUserChanged(string value) => RefreshQueryPreview();
    partial void OnOrgChanged(string value) => RefreshQueryPreview();
    partial void OnExtraQualifiersChanged(string value) => RefreshQueryPreview();
    partial void OnMinStarsChanged(int value) => RefreshQueryPreview();
    partial void OnCreatedWithinHoursChanged(int value) => RefreshQueryPreview();
    partial void OnExcludeForksChanged(bool value) => RefreshQueryPreview();
    partial void OnExcludeArchivedChanged(bool value) => RefreshQueryPreview();
    partial void OnIsRunningChanged(bool value) => OnPropertyChanged(nameof(StartStopLabel));

    [RelayCommand]
    private async Task ToggleMonitorAsync()
    {
        if (IsRunning) Stop();
        else await StartAsync();
    }

    [RelayCommand]
    /// <summary>
    /// Polls the GitHub repositories once asynchronously.
    /// </summary>
    private async Task PollOnceAsync()
    {
        if (IsBusy) return;
        PersistSettings();
        IsBusy = true;
        try
        {
            await PollAsync(notify: true, seedOnly: false, CancellationToken.None);
        }
        catch (Exception ex)
        {
            StatusText = "Poll failed.";
            Log("Poll failed: " + ex.Message);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    /// <summary>
    /// Clears the seen repositories and the repository list, and logs the action.
    /// </summary>
    private void ClearSeen()
    {
        _seen.Clear();
        Repositories.Clear();
        Log("Cleared seen history.");
    }

    [RelayCommand]
    /// <summary>
    /// Clears the log lines.
    ///</summary>   
    private void ClearLog() => LogLines.Clear();

    [RelayCommand]
    /// <summary>
    /// Opens the selected repository in the default web browser.
    /// </summary>
    private void OpenSelected()
    {
        if (SelectedRepository is { } repo)
            _urls.Open(repo.HtmlUrl);
    }

    [RelayCommand]
    /// <summary>
    /// Quits the application by shutting down the current App instance.
    /// </summary>
    private void Quit()
    {
        App.Current.Shutdown();
    }

    [RelayCommand]
    /// <summary>
    /// Shows all sections by setting their visibility properties to true.
    /// </summary>
    private void ShowAllSections()
    {
        IsCriteriaVisible = true;
        IsResultsVisible = true;
        IsLogVisible = true;
    }

    [RelayCommand]

    /// <summary>
    /// Applies the specified layout state to the view model.
    /// </summary>
    /// <param name="state">The layout state to apply.</param>
    private void ApplyLayout(LayoutState state)
    {
        if (state.CriteriaWidth is >= 220 and <= 900)
            CriteriaWidth = state.CriteriaWidth.Value;
        if (state.LogHeight is >= 60 and <= 600)
            LogHeight = state.LogHeight.Value;
        PersistSettings();
    }

    [RelayCommand]
    /// <summary>
    /// Handles the closing of the application by stopping the monitor and persisting settings.
    /// </summary>
    private void Closing()
    {
        Stop();
        PersistSettings();
    }

    /// <summary>
    /// Starts the monitoring process by persisting settings, initializing a cancellation token, and running the polling loop asynchronously.
    /// </summary>
    /// <returns></returns>
    private async Task StartAsync()
    {
        PersistSettings();
        _cts = new CancellationTokenSource();
        IsRunning = true;
        StatusText = "Monitoring…";
        _toasts.ShowInfo("GitHub monitor started", "Watching for new repositories.");
        Log("Started.");
        var ct = _cts.Token;
        _ = Task.Run(() => RunLoopAsync(ct), ct);
    }

    /// <summary>
    /// Stops the monitoring process by canceling the cancellation token, resetting the running state, and updating the status text and log.
    /// </summary>
    private void Stop()
    {
        _cts?.Cancel();
        _cts = null;
        IsRunning = false;
        StatusText = "Stopped.";
        Log("Stopped.");
    }

    /// <summary>
    /// Runs the main polling loop asynchronously, periodically checking for new repositories based on the current settings and notifying the user as appropriate.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task RunLoopAsync(CancellationToken ct)
    {
        try
        {
            var settings = Snapshot();
            await PollAsync(notify: settings.NotifyOnStartSeed, seedOnly: !settings.NotifyOnStartSeed, ct);

            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(Math.Max(1, settings.PollIntervalMinutes)), ct);
                settings = Snapshot();
                await PollAsync(notify: true, seedOnly: false, ct);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                Log("Monitor error: " + ex.Message);
                _toasts.ShowInfo("GitHub monitor error", ex.Message);
                Stop();
            });
        }
    }

    /// <summary>
    /// Polls GitHub for new repositories based on the current settings, updates the UI with the results, and optionally notifies the user of new repositories found.
    /// </summary>
    /// <param name="notify">Whether to notify the user of new repositories found.</param>
    /// <param name="seedOnly">Whether to only seed the initial set of repositories without notifying.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task PollAsync(bool notify, bool seedOnly, CancellationToken ct)
    {
        var settings = Snapshot();
        var query = _search.BuildQuery(settings);

        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            QueryPreview = query;
            Log($"Polling: {query}");
        });

        var items = await _search.SearchAsync(settings, ct);
        var fresh = items.Where(r => _seen.IsNew(r.Id)).ToList();
        foreach (var repo in fresh)
            _seen.MarkSeen(repo.Id);
        _seen.Persist();

        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            foreach (var repo in fresh)
                Repositories.Insert(0, repo);

            StatusText = $"{(IsRunning ? "Monitoring" : "Idle")} · {DateTime.Now:t} · {fresh.Count} new / {items.Count} hits";
            Log($"Done. {items.Count} hits, {fresh.Count} new.");
        });

        if (notify && !seedOnly)
        {
            foreach (var repo in fresh)
                _toasts.ShowNewRepo(repo);
        }
    }

    /// <summary>
    /// Creates a snapshot of the current settings in a thread-safe manner by invoking on the UI dispatcher. 
    /// This snapshot is used for polling and other operations to ensure consistency of settings during asynchronous operations.
    /// </summary>
    /// <returns></returns>
    private MonitorSettings Snapshot() => App.Current.Dispatcher.Invoke(() => new MonitorSettings
    {
        Token = Token,
        Keywords = Keywords,
        Language = Language,
        Topic = Topic,
        User = User,
        Org = Org,
        ExtraQualifiers = ExtraQualifiers,
        MinStars = MinStars,
        CreatedWithinHours = CreatedWithinHours,
        PollIntervalMinutes = PollIntervalMinutes,
        MaxResultsPerPoll = MaxResultsPerPoll,
        ExcludeForks = ExcludeForks,
        ExcludeArchived = ExcludeArchived,
        NotifyOnStartSeed = NotifyOnStartSeed,
        IsCriteriaVisible = IsCriteriaVisible,
        IsResultsVisible = IsResultsVisible,
        IsLogVisible = IsLogVisible,
        CriteriaWidth = CriteriaWidth,
        LogHeight = LogHeight
    });

    /// <summary>
    /// Loads the settings from the provided <see cref="MonitorSettings"/> instance into the view model's properties. 
    /// This method is used during initialization to restore previously saved settings.
    /// </summary>
    /// <param name="s"></param>
    private void LoadFrom(MonitorSettings s)
    {
        Token = s.Token;
        Keywords = s.Keywords;
        Language = s.Language;
        Topic = s.Topic;
        User = s.User;
        Org = s.Org;
        ExtraQualifiers = s.ExtraQualifiers;
        MinStars = s.MinStars;
        CreatedWithinHours = s.CreatedWithinHours;
        PollIntervalMinutes = s.PollIntervalMinutes;
        MaxResultsPerPoll = s.MaxResultsPerPoll;
        ExcludeForks = s.ExcludeForks;
        ExcludeArchived = s.ExcludeArchived;
        NotifyOnStartSeed = s.NotifyOnStartSeed;
        IsCriteriaVisible = s.IsCriteriaVisible;
        IsResultsVisible = s.IsResultsVisible;
        IsLogVisible = s.IsLogVisible;
        CriteriaWidth = s.CriteriaWidth;
        LogHeight = s.LogHeight;
    }

    /// <summary>
    /// Persists the current settings to the settings store.
    /// </summary>
    private void PersistSettings() => _settingsStore.Save(Snapshot());

    /// <summary>
    /// Refreshes the query preview based on the current search criteria.
    /// </summary>
    private void RefreshQueryPreview() => QueryPreview = _search.BuildQuery(new MonitorSettings
    {
        Keywords = Keywords,
        Language = Language,
        Topic = Topic,
        User = User,
        Org = Org,
        ExtraQualifiers = ExtraQualifiers,
        MinStars = MinStars,
        CreatedWithinHours = CreatedWithinHours,
        ExcludeForks = ExcludeForks,
        ExcludeArchived = ExcludeArchived
    });

    /// <summary>
    /// Logs a message to the log lines collection with a timestamp. This method ensures that the log is updated on the UI thread.
    /// </summary>
    /// <param name="message"></param>
    private void Log(string message)
    {
        void Add() => LogLines.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (App.Current.Dispatcher.CheckAccess()) Add();
        else App.Current.Dispatcher.Invoke(Add);
    }
}