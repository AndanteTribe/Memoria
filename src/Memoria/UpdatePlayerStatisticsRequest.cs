using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// プレイヤーの統計情報を更新するリクエスト.
/// </summary>
public record UpdatePlayerStatisticsRequest(string TitleId, string XAuthorization, StatisticUpdate[] Statistics)
{
    [JsonIgnore]
    public string TitleId { get; } = TitleId;

    [JsonIgnore]
    public string XAuthorization { get; } = XAuthorization;

    /// <summary>
    /// 統計情報の配列.
    /// </summary>
    public StatisticUpdate[] Statistics { get; } = Statistics;
}

/// <summary>
/// 統計情報の更新内容.
/// </summary>
public record StatisticUpdate
{
    /// <summary>
    /// 統計情報の名前.
    /// </summary>
    public string StatisticName { get; init; } = string.Empty;

    /// <summary>
    /// 統計情報の値.
    /// </summary>
    public int Value { get; init; }
}