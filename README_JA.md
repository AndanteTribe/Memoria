# Memoria

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

[English version here](README.md)

## 概要

**Memoria** は、PlayFab のランキングとスコアを管理するためのクライアントサイド .NET ライブラリです。
ゲーム開発者が PlayFab のランキングシステムを最小限のコードで簡単に利用できるよう設計されています。

### 特徴

-  **シンプルな API** — `RankingClient` だけでログイン・スコア送信・ランキング取得が完結
-  **自動リトライ** — タイムアウトや一時的なサーバーエラー時に自動でリトライ（回数・間隔を設定可能）
-  **テスタブル** — `PlayFabClient` にカスタム `HttpMessageHandler` を注入可能。モックを使った単体テストが容易
-  **マルチターゲット** — `netstandard2.1` / `net8.0` 対応。Unity 2022.3 以降でも利用可能

## 動作要件

- .NET Standard 2.1 以上、または .NET 8.0 以上
- Unity 2022.3 以降（UPM 経由でインストールする場合）
- PlayFab のタイトル ID（PlayFab ダッシュボードから取得）

## インストール

### NuGet（.NET プロジェクト向け）

```bash
dotnet add package Memoria
```

### Unity Package Manager（Unity 向け）

Unity のパッケージマネージャーで、以下の Git URL を追加してください。

```
https://github.com/AndanteTribe/Memoria.git?path=bin/Memoria
```

## クイックスタート

`RankingClient` クラスを使用して、PlayFab のランキングシステムにアクセスできます。

### 1. 初期化とログイン

`RankingClient` を初期化する際には、PlayFab のタイトル ID を指定します。
`LoginAsync` を呼び出すことで、カスタム ID を使って PlayFab にログインします。

```csharp
using Memoria;

var rankingClient = new RankingClient("YourPlayFabTitleId");

// PlayFab にログイン
await rankingClient.LoginAsync(cancellationToken);
```

### 2. スコアの送信

`SendAsync` メソッドで、プレイヤー名とスコアをランキングに送信します。
第3引数の `statisticsName` を省略した場合、デフォルトで `"HighScore"` が使用されます。

```csharp
// ランキングにスコアを送信
await rankingClient.SendAsync("PlayerName", 1500, "HighScore", cancellationToken);
```

### 3. ランキングの取得

`LoadAsync` メソッドでランキングデータを取得します。
戻り値は `(string playerName, int score)` のタプル配列で、昇順（順位順）に並んでいます。

```csharp
// ランキングを取得（デフォルト: 上位10件）
var leaderboard = await rankingClient.LoadAsync("HighScore", 10, cancellationToken);

foreach (var entry in leaderboard)
{
    Console.WriteLine($"{entry.playerName}: {entry.score}");
}
```

## PlayFabClient を直接使用する

より細かい制御が必要な場合は、`PlayFabClient` を直接使用できます。
`PlayFabClient` は以下の3つのインターフェースを実装しています。

| インターフェース | 説明 | 主なメソッド |
|---|---|---|
| `IAuthentication` | 認証 | `LoginWithCustomIdAsync`, `LoginAndGetUserOptionAsync` |
| `IAccountManagement` | アカウント管理 | `UpdateUserTitleDisplayNameAsync` |
| `IPlayerDataManagement` | プレイヤーデータ管理 | `UpdatePlayerStatisticsAsync`, `GetLeaderboardAsync` |

### 使用例

```csharp
using Memoria;

using var client = new PlayFabClient();

// ログインしてユーザーオプションを取得
var loginRequest = new LoginWithCustomIdRequest("YourTitleId")
{
    CustomId = Guid.NewGuid(),
    CreateAccount = true,
};
var userOption = await client.Authentication.LoginAndGetUserOptionAsync(loginRequest);

// 表示名を更新
var nameRequest = new UpdateUserTitleDisplayNameRequest("PlayerName");
await client.AccountManagement.UpdateUserTitleDisplayNameAsync(nameRequest, userOption);

// スコアを送信
var statistics = new[]
{
    new StatisticUpdate { StatisticName = "HighScore", Value = 2000 }
};
await client.PlayerDataManagement.UpdatePlayerStatisticsAsync(
    new UpdatePlayerStatisticsRequest(statistics), userOption);

// リーダーボードを取得
var leaderboardRequest = new GetLeaderboardRequest(0, "HighScore", 10);
var response = await client.PlayerDataManagement.GetLeaderboardAsync(leaderboardRequest, userOption);

foreach (var entry in response.Result.Leaderboard)
{
    Console.WriteLine($"{entry.Position + 1}位: {entry.DisplayName} - {entry.StatValue}点");
}
```

### リトライ設定

`PlayFabClient` は、一時的なエラー（408, 429, 500, 502, 503, 504）やタイムアウト時に自動でリトライします。
リトライ回数と間隔はプロパティで設定できます。

```csharp
var client = new PlayFabClient();
client.MaxRetries = 5;                          // 最大リトライ回数（デフォルト: 3）
client.RetryDelay = TimeSpan.FromSeconds(2);    // リトライ間隔（デフォルト: 1秒）
```

### カスタム HttpMessageHandler の注入

テスト時にモックハンドラを注入することで、実際の HTTP 通信なしに動作を検証できます。

```csharp
var mockHandler = new MockHttpMessageHandler();
var client = new PlayFabClient(mockHandler);
var rankingClient = new RankingClient("TitleId", client);
```

## ライセンス

Memoria は [MIT ライセンス](LICENSE)の下で提供されています。
