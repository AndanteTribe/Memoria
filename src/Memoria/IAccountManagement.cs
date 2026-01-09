namespace Memoria;

/// <summary>
/// アカウント管理機能を提供します.
/// </summary>
public interface IAccountManagement
{
    /// <summary>
    ///　プレイヤーの表示名を更新します.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<UpdateUserTitleDisplayNameResponse> UpdateUserTitleDisplayNameAsync(UpdateUserTitleDisplayNameRequest request, CancellationToken cancellationToken = default);
}