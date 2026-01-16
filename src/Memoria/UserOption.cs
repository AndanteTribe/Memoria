using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// ユーザーオプション情報.
/// </summary>
/// <param name="LoginResult"></param>
/// <param name="TitleId"></param>
public record UserOption([property:JsonPropertyName("data")]LoginResult LoginResult, string TitleId);
