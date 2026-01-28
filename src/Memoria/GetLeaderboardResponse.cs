using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// リーダーボード取得レスポンス.
/// </summary>
public record GetLeaderboardResponse
{
    /// <summary>
    /// リーダーボード取得結果.
    /// </summary>
    [JsonPropertyName("data")]
    public GetLeaderboardResult Result { get; init; } = null!;
}

/// <summary>
/// リーダーボード取得結果.
/// </summary>
public record GetLeaderboardResult
{
    /// <summary>
    /// リーダーボードエントリーの配列.
    /// </summary>
    [JsonPropertyName("Leaderboard")]
    public PlayerLeaderboardEntry[] Leaderboard { get; init; } = null!;
}

/// <summary>
/// プレイヤーのリーダーボードエントリー.
/// </summary>
public record PlayerLeaderboardEntry
{
    /// <summary>
    /// 表示名.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// PlayFab ID.
    /// </summary>
    public string PlayFabId { get; init; } = string.Empty;

    /// <summary>
    /// 順位.
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    /// 統計情報の値.
    /// </summary>
    public int StatValue { get; init; }
}