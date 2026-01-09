namespace Memoria;

/// <summary>
/// 認証関連の操作を提供する.
/// </summary>
public interface IAuthentication
{
    /// <summary>
    /// カスタムIDを使用してログインする.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<LoginWithCustomIdResponse> LoginWithCustomIdAsync(LoginWithCustomIdRequest request, CancellationToken cancellationToken = default);
}