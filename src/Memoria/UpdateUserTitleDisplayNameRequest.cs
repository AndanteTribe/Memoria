namespace Memoria;

/// <summary>
/// ユーザーのタイトル表示名を更新するリクエスト.
/// </summary>
/// <param name="DisplayName">表示名</param>
public record UpdateUserTitleDisplayNameRequest(string DisplayName);