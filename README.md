<img width="344" height="135" alt="layer-app-icon (1)" src="https://github.com/user-attachments/assets/834a39a7-1c47-4d4b-953f-d1385561148e" />


GitHubMonitor is a Windows Presentation Foundation (WPF) application developed in C#. It continuously monitors GitHub for newly created repositories and provides real-time notifications. Users can define custom criteria to filter and display only the repositories that match their specified criteria. Click a toast or double-click a row to open the repo in the browser.

<img width="1170" height="910" alt="image" src="https://github.com/user-attachments/assets/db556a68-1b48-4534-bed7-d0d444b8b568" />


Stack: .NET (Windows TFM), WPF, MVVM (`CommunityToolkit.Mvvm`), GitHub Search API, WinRT app notifications.

## Table of Contents
1. [Features](#features)
1. [Requirements](#requirements)
1. [Build and Run](#build-and-run)
1. [How to Use](#how-to-use)
1. [View Menu](#view-menu)
1. [Criteria Mapped to GitHub Search](#criteria-mapped-to-github-search)
1. [Toast Notifications](#toast-notifications)
1. [Files](#files)
1. [Project Layout](#project-layout)
1. [Limits](#limits)
1. [License](#license)

## Features

- Criteria panel: keywords, language, topic, user/org, stars, age, extra GitHub qualifiers, PAT
- Background polling with start/stop and a one-shot **Poll once**
- Deduplicated results (seen repo IDs persisted)
- Windows toast on each new match (`ToastGeneric`; click opens the repo URL)
- **View** menu to show or hide Criteria, Results, and Log
- Grid splitters to resize Criteria vs Results and Results vs Log
- Hidden Criteria: results and log expand to full width
- Hidden Log: New matches expands to fill the vertical space
- Log panel with **Clear log** and auto-scroll (stays put if you scroll up)
- Layout, visibility, and criteria saved under `%LocalAppData%\GitHubNewRepoMonitor\`


## Requirements

- Windows 10 version 2004+ or Windows 11
- .NET SDK that can target `net8.0-windows10.0.19041.0` or `net10.0-windows10.0.19041.0`
- Network access to `https://api.github.com`
- Optional: [GitHub personal access token](https://github.com/settings/tokens)

Anonymous search is limited to **10 requests/minute**. A token raises that to **30 requests/minute**.

## Build and Run

The project file must use a Windows 10 SDK TFM so toast APIs exist:


```xml
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
<UseWPF>true</UseWPF>
```

```powershell
dotnet restore
dotnet build
dotnet run
```

NuGet packages:
* CommunityToolkit.Mvvm
* Microsoft.Extensions.DependencyInjection

Do not set StartupUri in App.xaml. The window is created in App.OnStartup and assigned a MainViewModel from DI.

## How to Use
1. Fill in Criteria. Watch the query preview; that string is what GitHub receives.
2. Paste a personal access token if you have one.
3. Click Start monitoring. The first poll seeds seen IDs (no toast flood) unless Toast on first poll too is checked.
4. New repositories appear under New matches and as toasts.
5. Double-click a row or click the toast to open the repository.
6. Poll once runs a single search immediately.
7. Clear seen history allows current matches to notify again.
8. Close the window to save settings and stop the loop.

## View Menu

| Item| Effect
| --- | --- |
| Criteria | Show or hide the left filter panel. When hidden, Results and Log use the full width.  
| Results | Show or hide New matches (and the log host).
| Log | Show or hide the log. When hidden, New matches uses the remaining height.
| Show All | Turns every section back on.

Drag the vertical splitter to resize Criteria. Drag the horizontal splitter to resize the log. Sizes are written to settings when you drag or close the window.

## Criteria Mapped to GitHub search 

| Field| Becomes
| --- | --- |
|Keywords|Free text
|Language|<mark>language:</mark>
|Topic |<mark>topic:</mark>
|User / Organization|<mark>user: / org:</mark>
|Min stars|<mark>stars:>=N</mark>
|Created within (hours)|<mark>created:>ISO-8601</mark>
|Extra qualifiers|Raw, for example <mark>topic:mcp stars:10..50</mark>
|Exclude forks / archived|<mark>fork:false archived:false</mark>

* Examples:New C# repos in the last day: Language C#, Created within 24
* Agent repos with traction: Keywords agent, Min stars 20

## Toast Notifications
Toasts use inbox WinRT APIs (<mark>Windows.UI.Notifications</mark>).

```csharp

ToastNotificationManager
    .CreateToastNotifier("GitHubNewRepoMonitor")
    .Show(toast);
```

The payload is <mark>ToastGeneric</mark> with <mark>activationType="protocol"</mark> and <mark>launch="https://github.com/..."</mark>.

If a toast does not appear:

1. Confirm the TFM is <mark>net*-windows10.0.19041.0</mark> (or another 10.0.17763+ SDK).
2. Open Settings → System → Notifications and allow banners for this app. The first toast registers the    AUMID <mark>GitHubNewRepoMonitor</mark>.
3. Turn off Focus assist / Do not disturb while testing.
4. Run the app as the logged-in user, not as LocalSystem.

## Files

Path: <mark>%LocalAppData%\GitHubMonitor\</mark>

| File| Contents|
| --- | --- |
| settings.json | Criteria, token, poll interval, section visibility, splitter sizes |
| seen.json | Repository IDs already processed |

The token is stored in plain text. Use a revocable, minimally scoped PAT.

## Project Layout

```Text
App.xaml / App.xaml.cs          DI, resources (BoolToVis, Label style)
Views/MainWindow.xaml           Layout, menu, splitters
Views/MainWindow.xaml.cs        Splitter persist, collapse columns/rows
ViewModels/MainViewModel.cs     Commands, polling, settings snapshot
Models/                         GitHubRepo, MonitorSettings, LayoutState
Services/                       Search, settings, seen IDs, toasts, URL open
Behaviors/ListBoxAutoScrollBehavior.cs
Converters/InverseBoolConverter.cs
```

## Limits

* GitHub search is eventually consistent.
* Keep the poll interval at 5+ minutes to stay under rate limits.
* Public repositories only unless the token can see more.
* Query length is capped by GitHub.

GitHub API terms apply to any token and traffic you generate.

## License

Copyright © Scott Cantwell

GitHubMonitor is provided as-is under the Apache 2.0 license. For more information see LICENSE.

























