using System.Text.Json.Serialization;

namespace Memoria;

/// <summary>
/// PlayFab Client JSON シリアライザーコンテキスト.
/// </summary>
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(LoginWithCustomIdResponse))]
[JsonSerializable(typeof(LoginWithCustomIdRequest))]
[JsonSerializable(typeof(UpdateUserTitleDisplayNameResponse))]
[JsonSerializable(typeof(UpdatePlayerStatisticsRequest))]
[JsonSerializable(typeof(StatisticUpdate))]
[JsonSerializable(typeof(LoginResult))]
[JsonSerializable(typeof(UpdateUserTitleDisplayNameResult))]
[JsonSerializable(typeof(GetLeaderboardResponse))]
[JsonSerializable(typeof(GetLeaderboardResult))]
[JsonSerializable(typeof(PlayerLeaderboardEntry))]
public partial class PlayFabClientJsonSerializerContext : JsonSerializerContext
{

}