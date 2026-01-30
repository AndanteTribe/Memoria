namespace Memoria;

/// <summary>
/// ランキングリポジトリ.
/// </summary>
public class RankingRepository : IRankingRepository
{
    private readonly string _titleId;
    private readonly PlayFabClient _client;
    private UserOption? _userOption;

    /// <summary>
    /// コンストラクタ.
    /// </summary>
    /// <param name="titleId"></param>
    /// <param name="client"></param>
    public RankingRepository(string titleId, PlayFabClient? client = null)
    {
        _titleId = titleId;
        _client = client ?? new PlayFabClient();
    }

    /// <summary>
    /// ランキングデータを送信する.
    /// </summary>
    /// <param name="rankingData"></param>
    /// <param name="cancellationToken"></param>
    public async ValueTask SendAsync(RankingData rankingData, CancellationToken cancellationToken = default)
    {
        var loginRequest = new LoginWithCustomIdRequest(_titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        _userOption ??= await _client.Authentication.LoginAndGetUserOptionAsync(loginRequest, cancellationToken);

        var nameRequest = new UpdateUserTitleDisplayNameRequest(rankingData.PlayerName);
        await _client.AccountManagement.UpdateUserTitleDisplayNameAsync(nameRequest, _userOption, cancellationToken);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = rankingData.StatisticName,
                Value = rankingData.Score
            }
        };

        await _client.PlayerDataManagement.UpdatePlayerStatisticsAsync(
            new UpdatePlayerStatisticsRequest(statistics),
            _userOption,
            cancellationToken
            );
    }

    /// <summary>
    /// ランキングデータをロードする.
    /// </summary>
    /// <param name="statisticName"></param>
    /// <param name="maxResultsCount"></param>
    /// <param name="cancellationToken"></param>
    public async ValueTask<RankingData[]> LoadAsync(
        string statisticName,
        uint maxResultsCount = 10,
        CancellationToken cancellationToken = default)
    {
        var loginRequest = new LoginWithCustomIdRequest(_titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        _userOption ??= await _client.Authentication.LoginAndGetUserOptionAsync(loginRequest, cancellationToken);
        var leaderboardRequest = new GetLeaderboardRequest(0, statisticName, maxResultsCount);
        var leaderboardResponse = await _client.PlayerDataManagement.GetLeaderboardAsync(
            leaderboardRequest,
            _userOption,
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