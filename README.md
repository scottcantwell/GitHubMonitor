# GitHubMonitor
GitHubMonitor is a Windows Presentation Foundation (WPF) application developed in C#. It continuously monitors GitHub for newly created repositories and provides real-time notifications. Users can define custom criteria to filter and display only the repositories that match their specified criteria. Click a toast or double-click a row to open the repo in the browser.

<img width="1190" height="920" alt="image" src="https://github.com/user-attachments/assets/db556a68-1b48-4534-bed7-d0d444b8b568" />






Stack: .NET (Windows TFM), WPF, MVVM (`CommunityToolkit.Mvvm`), GitHub Search API, WinRT app notifications.


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

## Build and run

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




