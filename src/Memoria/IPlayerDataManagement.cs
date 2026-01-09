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
    /// <param name="titleId"></param>
    /// <param name="xAuthorization"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<UpdatePlayerStatisticsResponse> UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request, string titleId, string xAuthorization, CancellationToken cancellationToken = default);

    /// <summary>
    /// プレイヤーの統計情報を更新する（ユーザーオプション付き）.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userOption"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<UpdatePlayerStatisticsResponse> UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request, UserOption userOption, CancellationToken cancellationToken = default);
}