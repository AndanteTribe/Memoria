namespace Memoria;

/// <summary>
/// プレイヤーのデータ管理に関するインターフェース
/// </summary>
public interface IPlayerDataManagement
{
    /// <summary>
    /// プレイヤーの統計情報を更新する.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<UpdatePlayerStatisticsResponse> UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request, CancellationToken cancellationToken = default);
}