using System.Net;
using System.Text.Json;

namespace Memoria.Tests;

/// <summary>
/// PlayFab Client APIのモック用HttpMessageHandler.
/// </summary>
public class MoqPlayFabHandler : DelegatingHandler
{
    /// <summary>
    /// 返却したいステータスコード（デフォルト200 OK）
    /// </summary>
    public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

    /// <summary>
    /// 返却したいレスポンスデータ.
    /// </summary>
    public object? ResponseData { get; set; }

    /// <summary>
    /// 最後に送信されたリクエストをここに保存する
    /// </summary>
    public HttpRequestMessage? LastRequest { get; private set; }

    /// <summary>
    /// 最後に送信されたリクエストボディをここに保存する
    /// </summary>
    public string? LastRequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // リクエスト内容を保存（Assertで確認するため）
        LastRequest = request;
        if (request.Content != null)
        {
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        var responseMessage = new HttpResponseMessage(ResponseStatusCode);

        if (ResponseData != null)
        {
            // オブジェクトをJSON文字列に変換
            var json = JsonSerializer.Serialize(ResponseData);
            responseMessage.Content = new StringContent(json);
        }
        else
        {
            // 空の場合は空のJSONオブジェクトを返す
            responseMessage.Content = new StringContent("{}");
        }

        return responseMessage;
    }
}