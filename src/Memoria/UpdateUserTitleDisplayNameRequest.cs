using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// ユーザーのタイトル表示名を更新するリクエスト.
/// </summary>
public record UpdateUserTitleDisplayNameRequest(string TitleId, string XAuthorization, string DisplayName)
{
    /// <summary>
    /// タイトルID.
    /// </summary>
    [JsonIgnore]
    public string TitleId { get; } = TitleId;

    /// <summary>
    /// 認証トークン.
    /// </summary>
    [JsonIgnore]
    public string XAuthorization { get; } = XAuthorization;

    /// <summary>
    /// 表示名.
    /// </summary>
    public string DisplayName { get; } = DisplayName;
}