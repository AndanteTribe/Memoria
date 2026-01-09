using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// カスタムIDでログインするリクエスト.
/// </summary>
public record LoginWithCustomIdRequest(string TitleId)
{
    /// <summary>
    /// タイトルID.
    /// </summary>
    public string TitleId { get; } = TitleId;

    /// <summary>
    /// カスタムID.
    /// </summary>
    [JsonConverter(typeof(GuidConverter))]
    public Guid CustomId { get; init; }

    /// <summary>
    /// アカウントを作成するかどうか.
    /// </summary>
    public bool CreateAccount { get; init; }
}