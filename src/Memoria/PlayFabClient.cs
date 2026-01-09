using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Memoria;

/// <summary>
///　PlayFabAPIクライアント
/// </summary>
public class PlayFabClient : IAccountManagement, IPlayerDataManagement, IAuthentication, IDisposable
{
    /// <summary>
    /// アカウント管理API
    /// </summary>
    public IAccountManagement AccountManagement => this;

    /// <summary>
    /// プレイヤーデータ管理API
    /// </summary>
    public IPlayerDataManagement PlayerDataManagement => this;

    /// <summary>
    /// 認証API
    /// </summary>
    public IAuthentication Authentication => this;

    /// <summary>
    /// 最大リトライ回数
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 内部で使用するHttpClient
    /// </summary>
    public HttpClient HttpClient => _httpClient;

    private readonly HttpClient _httpClient;

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
            null,
            cancellationToken);
        return response;
    }

    async ValueTask<UpdateUserTitleDisplayNameResponse> IAccountManagement.UpdateUserTitleDisplayNameAsync(
        UpdateUserTitleDisplayNameRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResponse>(
            request.TitleId,
            "/Client/UpdateUserTitleDisplayName",
            request,
            request.XAuthorization,
            cancellationToken);
        return response;
    }

    async ValueTask<UpdatePlayerStatisticsResponse> IPlayerDataManagement.UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SendPlayFabRequestAsync<UpdatePlayerStatisticsRequest, UpdatePlayerStatisticsResponse>(
            request.TitleId,
            "/Client/UpdatePlayerStatistics",
            request,
            request.XAuthorization,
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
        string? authHeader,
        CancellationToken cancellationToken)
    {
        var url = $"https://{titleId}.playfabapi.com{endpointPath}";
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(requestBody);

        for(int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var message = new HttpRequestMessage(HttpMethod.Post, url);
                using var content = new ByteArrayContent(jsonBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(authHeader))
                {
                    message.Headers.Add("X-Authorization", authHeader);
                }

                message.Content = content;

                using var response = await _httpClient.SendAsync(message, cancellationToken);

                // 成功時の処理
                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsByteArrayAsync();
                    return JsonSerializer.Deserialize<TResponse>(responseBytes)!;
                }

                // リトライ可能なエラーの場合、リトライ
                if (ShouldRetry(response.StatusCode) && attempt < MaxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
                    continue;
                }

                var errorDetail = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"PlayFab API Error ({response.StatusCode}): {errorDetail}");
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested &&
                                                attempt < MaxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(HttpClient.Timeout.Seconds), cancellationToken);
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