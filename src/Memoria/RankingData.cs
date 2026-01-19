namespace Memoria;

/// <summary>
/// ランキングデータクラス.
/// </summary>
/// <typeparam name="T"></typeparam>
public record RankingData<T>
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
    public T Score { get; set; } = default!;
}