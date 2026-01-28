using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// ユーザーのタイトル表示名を更新するレスポンス.
/// </summary>
public record UpdateUserTitleDisplayNameResponse
{
    /// <summary>
    /// レスポンスデータ.
    /// </summary>
    [JsonPropertyName("data")]
    public UpdateUserTitleDisplayNameResult Result { get; init; } = null!;
}

/// <summary>
/// ユーザーのタイトル表示名更新結果.
/// </summary>
public record UpdateUserTitleDisplayNameResult
{
    /// <summary>
    /// 表示名.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;
}