using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Memoria;

/// <summary>
/// PlayFabAPIクライアント
/// </summary>
public class PlayFabClient : IAuthentication, IAccountManagement, IPlayerDataManagement, IDisposable
{
    /// <summary>
    /// 認証API
    /// </summary>
    public IAuthentication Authentication => this;

    /// <summary>
    /// アカウント管理API
    /// </summary>
    public IAccountManagement AccountManagement => this;

    /// <summary>
    /// プレイヤーデータ管理API
    /// </summary>
    public IPlayerDataManagement PlayerDataManagement => this;

    /// <summary>
    /// 最大リトライ回数
    /// </summary>
    public uint MaxRetries { get; set; } = 3;

    /// <summary>
    /// リトライ間隔
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// 内部で使用するHttpClient
    /// </summary>
    public HttpClient HttpClient => _httpClient;

    private readonly HttpClient _httpClient;
    private readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new("application/json");

    /// <summary>
    /// デフォルトコンストラクタ
    /// </summary>
    public PlayFabClient() : this(new HttpClientHandler())
    {
    }

    /// <summary>
    /// カスタムハンドラを使用するコンストラクタ
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="disposeHandler"></param>
    public PlayFabClient(HttpClientHandler handler, bool disposeHandler = true)
    {
        _httpClient = new HttpClient(handler, disposeHandler);
    }

    async ValueTask<LoginWithCustomIdResponse> IAuthentication.LoginWithCustomIdAsync(LoginWithCustomIdRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<LoginWithCustomIdRequest, LoginWithCustomIdResponse>(
            request.TitleId,
            "/Client/LoginWithCustomID",
            request,
            string.Empty,
            cancellationToken);
        return response;
    }

    async ValueTask<UserOption> IAuthentication.LoginAndGetUserOptionAsync(LoginWithCustomIdRequest request,
        CancellationToken cancellationToken)
    {
        var response = await Authentication.LoginWithCustomIdAsync(request, cancellationToken);
        var userOption = new UserOption(response.Result, request.TitleId);
        return userOption;
    }

    async ValueTask<UpdateUserTitleDisplayNameResponse> IAccountManagement.UpdateUserTitleDisplayNameAsync(
        UpdateUserTitleDisplayNameRequest request,
        string titleId,
        string xAuthorization,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResponse>(
            titleId,
            "/Client/UpdateUserTitleDisplayName",
            request,
            xAuthorization,
            cancellationToken);
        return response;
    }

    async ValueTask<UpdateUserTitleDisplayNameResponse> IAccountManagement.UpdateUserTitleDisplayNameAsync(
        UpdateUserTitleDisplayNameRequest request,
        UserOption userOption,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResponse>(
            userOption.TitleId,
            "/Client/UpdateUserTitleDisplayName",
            request,
            userOption.LoginResult.SessionTicket,
            cancellationToken);
        return response;
    }

    async ValueTask<UpdatePlayerStatisticsResponse> IPlayerDataManagement.UpdatePlayerStatisticsAsync(
        UpdatePlayerStatisticsRequest request,
        string titleId,
        string xAuthorization,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<UpdatePlayerStatisticsRequest, UpdatePlayerStatisticsResponse>(
            titleId,
            "/Client/UpdatePlayerStatistics",
            request,
            xAuthorization,
            cancellationToken);
        return response;
    }

    async ValueTask<UpdatePlayerStatisticsResponse> IPlayerDataManagement.UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request,
        UserOption userOption,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<UpdatePlayerStatisticsRequest, UpdatePlayerStatisticsResponse>(
            userOption.TitleId,
            "/Client/UpdatePlayerStatistics",
            request,
            userOption.LoginResult.SessionTicket,
            cancellationToken);
        return response;
    }

    /// <summary>
    /// PlayFabへのHTTPリクエストを処理する汎用メソッド
    /// </summary>
    private async ValueTask<TResponse> SendPlayFabRequestAsync<TRequest, TResponse>(
        string titleId,
        string endpointPath,
        TRequest requestBody,
        string authHeader,
        CancellationToken cancellationToken)
    {
        var url = "https://" + titleId + ".playfabapi.com" + endpointPath;
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(requestBody);

        for(var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                using var message = new HttpRequestMessage(HttpMethod.Post, url);
                using var content = new ByteArrayContent(jsonBytes);

                content.Headers.ContentType = _mediaTypeHeaderValue;

                if (!string.IsNullOrEmpty(authHeader))
                {
                    message.Headers.Add("X-Authorization", authHeader);
                }

                message.Content = content;

                using var response = await _httpClient.SendAsync(message, cancellationToken);

                // 成功時の処理
                if (response.IsSuccessStatusCode)
                {
#if NET8_0_OR_GREATER
                    var responseBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    return JsonSerializer.Deserialize<TResponse>(responseBytes)!;
#else
                    cancellationToken.ThrowIfCancellationRequested();
                    var responseString = await response.Content.ReadAsStringAsync();
                    cancellationToken.ThrowIfCancellationRequested();
                    return JsonSerializer.Deserialize<TResponse>(responseString)!;
#endif
                }

                // リトライ可能なエラーの場合、リトライ
                if (ShouldRetry(response.StatusCode) && attempt < MaxRetries)
                {
                    await Task.Delay(RetryDelay, cancellationToken);
                    continue;
                }
#if NET8_0_OR_GREATER
                var errorDetail = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(
                    $"PlayFab API Error ({response.StatusCode}): {errorDetail}");
#else
                cancellationToken.ThrowIfCancellationRequested();
                var errorDetail = await response.Content.ReadAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();
                throw new HttpRequestException(
                    $"PlayFab API Error ({response.StatusCode}): {errorDetail}");
#endif
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested &&
                                                attempt < MaxRetries - 1)
            {
                // タイムアウトの場合、リトライ
                await Task.Delay(RetryDelay, cancellationToken);
            }
        }
        throw new TimeoutException("PlayFab request failed after maximum retry attempts.");
    }

    private static bool ShouldRetry(HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.RequestTimeout => true,        // 408
            (HttpStatusCode)429 => true,                  // 429
            HttpStatusCode.InternalServerError => true,   // 500
            HttpStatusCode.BadGateway => true,            // 502
            HttpStatusCode.ServiceUnavailable => true,    // 503
            HttpStatusCode.GatewayTimeout => true,        // 504
            _ => false
        };

    /// <inheritdoc />
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}