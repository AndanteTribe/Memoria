# Memoria

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

[日本語版はこちら](README_JA.md)

## Overview

**Memoria** is a client-side .NET library for managing PlayFab rankings and scores.  
It is designed to allow game developers to easily integrate PlayFab's leaderboard system with minimal code.

### Features

-  **Simple API** — Login, submit scores, and fetch leaderboards all through `RankingClient`
-  **Auto Retry** — Automatically retries on timeouts and transient server errors (configurable retry count and interval)
-  **Testable** — Inject a custom `HttpMessageHandler` into `PlayFabClient` for easy unit testing with mocks
-  **Multi-Target** — Supports `netstandard2.1` / `net8.0`. Also compatible with Unity 2022.3+

## Requirements

- .NET Standard 2.1 or later, or .NET 8.0 or later
- Unity 2022.3 or later (when installing via UPM)
- A PlayFab Title ID (obtained from the PlayFab dashboard)

## Installation

### NuGet (for .NET projects)

```bash
dotnet add package Memoria
```

### Unity Package Manager (for Unity)

Add the following Git URL in the Unity Package Manager:

```
https://github.com/AndanteTribe/Memoria.git?path=bin/Memoria
```

## Quick Start

Use the `RankingClient` class to interact with PlayFab's leaderboard system.

### 1. Initialization and Login

Specify your PlayFab Title ID when initializing `RankingClient`.  
Call `LoginAsync` to log in to PlayFab using a custom ID.

```csharp
using Memoria;

var rankingClient = new RankingClient("YourPlayFabTitleId");

// Log in to PlayFab
await rankingClient.LoginAsync(cancellationToken);
```

### 2. Submitting a Score

Use the `SendAsync` method to submit a player name and score to the leaderboard.  
If the third argument `statisticsName` is omitted, it defaults to `"HighScore"`.

```csharp
// Submit a score to the leaderboard
await rankingClient.SendAsync("PlayerName", 1500, "HighScore", cancellationToken);
```

### 3. Fetching the Leaderboard

Use the `LoadAsync` method to retrieve leaderboard data.  
The return value is a tuple array of `(string playerName, int score)`, sorted in ascending order by rank.

```csharp
// Fetch the leaderboard (default: top 10)
var leaderboard = await rankingClient.LoadAsync("HighScore", 10, cancellationToken);

foreach (var entry in leaderboard)
{
    Console.WriteLine($"{entry.playerName}: {entry.score}");
}
```

## Using PlayFabClient Directly

For more granular control, you can use `PlayFabClient` directly.  
`PlayFabClient` implements the following three interfaces:

| Interface | Description | Key Methods |
|---|---|---|
| `IAuthentication` | Authentication | `LoginWithCustomIdAsync`, `LoginAndGetUserOptionAsync` |
| `IAccountManagement` | Account management | `UpdateUserTitleDisplayNameAsync` |
| `IPlayerDataManagement` | Player data management | `UpdatePlayerStatisticsAsync`, `GetLeaderboardAsync` |

### Example

```csharp
using Memoria;

using var client = new PlayFabClient();

// Log in and obtain user options
var loginRequest = new LoginWithCustomIdRequest("YourTitleId")
{
    CustomId = Guid.NewGuid(),
    CreateAccount = true,
};
var userOption = await client.Authentication.LoginAndGetUserOptionAsync(loginRequest);

// Update display name
var nameRequest = new UpdateUserTitleDisplayNameRequest("PlayerName");
await client.AccountManagement.UpdateUserTitleDisplayNameAsync(nameRequest, userOption);

// Submit a score
var statistics = new[]
{
    new StatisticUpdate { StatisticName = "HighScore", Value = 2000 }
};
await client.PlayerDataManagement.UpdatePlayerStatisticsAsync(
    new UpdatePlayerStatisticsRequest(statistics), userOption);

// Fetch the leaderboard
var leaderboardRequest = new GetLeaderboardRequest(0, "HighScore", 10);
var response = await client.PlayerDataManagement.GetLeaderboardAsync(leaderboardRequest, userOption);

foreach (var entry in response.Result.Leaderboard)
{
    Console.WriteLine($"#{entry.Position + 1}: {entry.DisplayName} - {entry.StatValue} pts");
}
```

### Retry Configuration

`PlayFabClient` automatically retries on transient errors (408, 429, 500, 502, 503, 504) and timeouts.  
The retry count and interval can be configured via properties.

```csharp
var client = new PlayFabClient();
client.MaxRetries = 5;                          // Max retry attempts (default: 3)
client.RetryDelay = TimeSpan.FromSeconds(2);    // Retry interval (default: 1 second)
```

### Injecting a Custom HttpMessageHandler

Inject a mock handler during testing to verify behavior without actual HTTP communication.

```csharp
var mockHandler = new MockHttpMessageHandler();
var client = new PlayFabClient(mockHandler);
var rankingClient = new RankingClient("TitleId", client);
```

## License

Memoria is provided under the [MIT License](LICENSE).
