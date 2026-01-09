namespace Memoria;

/// <summary>
/// アカウント管理機能を提供します.
/// </summary>
public interface IAccountManagement
{
    /// <summary>
    /// プレイヤーの表示名を更新します.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="titleId"></param>
    /// <param name="xAuthorization"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<UpdateUserTitleDisplayNameResponse> UpdateUserTitleDisplayNameAsync(UpdateUserTitleDisplayNameRequest request, string titleId, string xAuthorization, CancellationToken cancellationToken = default);

    /// <summary>
    /// プレイヤーの表示名を更新します(ユーザーオプション付き).
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userOption"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<UpdateUserTitleDisplayNameResponse> UpdateUserTitleDisplayNameAsync(UpdateUserTitleDisplayNameRequest request, UserOption userOption, CancellationToken cancellationToken = default);
}