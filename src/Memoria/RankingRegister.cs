namespace Memoria;

/// <summary>
/// ランキング登録クラス.
/// </summary>
public class RankingRegister
{
    private readonly string _titleId;
    private readonly PlayFabClient _client;
    private UserOption? _userOption;

    /// <summary>
    /// コンストラクタ.
    /// </summary>
    /// <param name="titleId"></param>
    /// <param name="client"></param>
    public RankingRegister(string titleId, PlayFabClient? client = null)
    {
        _titleId = titleId;
        _client = client ?? new PlayFabClient();
    }

    /// <summary>
    /// ランキングデータを送信する.
    /// </summary>
    /// <param name="rankingData"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    public async ValueTask SendAsync<T>(RankingData<T> rankingData, CancellationToken cancellationToken = default)
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
                Value = rankingData.Score!.ToString()!
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
    /// <typeparam name="T"></typeparam>
    public async ValueTask<RankingData<T>[]> LoadAsync<T>(
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

        var results = new RankingData<T>[leaderboardResponse.Result.Leaderboard.Length];
        for (int i = 0; i < leaderboardResponse.Result.Leaderboard.Length; i++)
        {
            var entry = leaderboardResponse.Result.Leaderboard[i];
            results[i] = new RankingData<T>
            {
                PlayerName = entry.DisplayName,
                StatisticName = statisticName,
                Score = entry.StatValue is T value ? value : default!,
            };
        }
        return results;
    }
}