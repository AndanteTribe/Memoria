namespace Memoria;

/// <summary>
/// ランキングデータクラス.
/// </summary>
public record RankingData
{
    /// <summary>
    /// 登録する際のプレイヤー名.
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// 統計情報の名前.
    /// </summary>
    public string StatisticName { get; set; } = "HighScore";

    /// <summary>
    /// スコアデータ.
    /// </summary>
    public int Score { get; set; }
}