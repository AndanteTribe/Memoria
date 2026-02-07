namespace Memoria;

/// <summary>
/// ランキングデータを送受信するクラス.
/// </summary>
public static class RankingClient
{
    /// <summary>
    /// PlayFabクライアント.
    /// </summary>
    public static PlayFabClient Client { get; set; } = new();
    private static UserOption? s_userOption;

    /// <summary>
    /// ランキングデータを送信する.
    /// </summary>
    /// <param name="titleId"></param>
    /// <param name="rankingData"></param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask SendAsync(string titleId, RankingData rankingData, CancellationToken cancellationToken = default)
    {
        var loginRequest = new LoginWithCustomIdRequest(titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        s_userOption ??= await Client.Authentication.LoginAndGetUserOptionAsync(loginRequest, cancellationToken);

        var nameRequest = new UpdateUserTitleDisplayNameRequest(rankingData.PlayerName);
        await Client.AccountManagement.UpdateUserTitleDisplayNameAsync(nameRequest, s_userOption, cancellationToken);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = rankingData.StatisticName,
                Value = rankingData.Score
            }
        };

        await Client.PlayerDataManagement.UpdatePlayerStatisticsAsync(
            new UpdatePlayerStatisticsRequest(statistics),
            s_userOption,
            cancellationToken
            );
    }

    /// <summary>
    /// ランキングデータをロードする.
    /// </summary>
    /// <param name="titleId"></param>
    /// <param name="statisticName"></param>
    /// <param name="maxResultsCount"></param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask<RankingData[]> LoadAsync(
        string titleId,
        string statisticName,
        uint maxResultsCount = 10,
        CancellationToken cancellationToken = default)
    {
        var loginRequest = new LoginWithCustomIdRequest(titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        s_userOption ??= await Client.Authentication.LoginAndGetUserOptionAsync(loginRequest, cancellationToken);
        var leaderboardRequest = new GetLeaderboardRequest(0, statisticName, maxResultsCount);
        var leaderboardResponse = await Client.PlayerDataManagement.GetLeaderboardAsync(
            leaderboardRequest,
            s_userOption,
            cancellationToken
            );

        var results = new RankingData[leaderboardResponse.Result.Leaderboard.Length];
        for (int i = 0; i < leaderboardResponse.Result.Leaderboard.Length; i++)
        {
            var entry = leaderboardResponse.Result.Leaderboard[i];
            results[i] = new RankingData
            {
                PlayerName = entry.DisplayName,
                StatisticName = statisticName,
                Score = entry.StatValue
            };
        }
        return results;
    }
}