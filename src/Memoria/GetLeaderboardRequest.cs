namespace Memoria;

/// <summary>
/// リーダーボード取得リクエスト.
/// </summary>
/// <param name="StartPosition"></param>
/// <param name="StatisticName"></param>
/// <param name="MaxResultsCount"></param>
public record GetLeaderboardRequest(int StartPosition, string StatisticName, uint MaxResultsCount = 10)
{
    /// <summary>
    /// 最大取得件数(最大100).
    /// </summary>
    public uint MaxResultsCount { get; init; } = MaxResultsCount > 100 ? 100 : MaxResultsCount;
}
