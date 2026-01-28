using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// リーダーボード取得レスポンス.
/// </summary>
public class GetLeaderboardResponse
{
    /// <summary>
    /// リーダーボード取得結果.
    /// </summary>
    [JsonPropertyName("data")]
    public GetLeaderboardResult Result { get; set; } = null!;
}

/// <summary>
/// リーダーボード取得結果.
/// </summary>
public class GetLeaderboardResult
{
    /// <summary>
    /// リーダーボードエントリーの配列.
    /// </summary>
    [JsonPropertyName("Leaderboard")]
    public PlayerLeaderboardEntry[] Leaderboard { get; set; } = null!;
}

/// <summary>
/// プレイヤーのリーダーボードエントリー.
/// </summary>
public class PlayerLeaderboardEntry
{
    /// <summary>
    /// 表示名.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// PlayFab ID.
    /// </summary>
    public string PlayFabId { get; set; } = string.Empty;

    /// <summary>
    /// 順位.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// 統計情報の値.
    /// </summary>
    public int StatValue { get; set; }
}