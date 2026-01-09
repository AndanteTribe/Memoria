using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// カスタムIDでログインするレスポンス.
/// </summary>
public record LoginWithCustomIdResponse
{
    /// <summary>
    /// レスポンスデータ.
    /// </summary>
    [JsonPropertyName("data")]
    public LoginResult Result { get; init; }
}

/// <summary>
/// ログイン結果.
/// </summary>
public record LoginResult
{
    /// <summary>
    /// 新規作成されたかどうか.
    /// </summary>
    public bool NewlyCreated { get; init; }

    /// <summary>
    /// セッショントークン.
    /// </summary>
    public string SessionTicket { get; init; } = string.Empty;
}