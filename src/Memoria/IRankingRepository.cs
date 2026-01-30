namespace Memoria;

/// <summary>
/// ランキングの登録と取得を行うリポジトリ.
/// </summary>
public interface IRankingRepository
{
    /// <summary>
    /// ランキングデータを送信する.
    /// </summary>
    /// <param name="rankingData"></param>
    /// <param name="cancellationToken"></param>
    ValueTask SendAsync(RankingData rankingData, CancellationToken cancellationToken = default);

    /// <summary>
    /// ランキングデータを取得する.
    /// </summary>
    /// <param name="statisticName"></param>
    /// <param name="maxResultsCount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<RankingData[]> LoadAsync(
        string statisticName,
        uint maxResultsCount = 10,
        CancellationToken cancellationToken = default);
}