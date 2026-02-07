namespace Memoria;

/// <summary>
/// ランキングデータを送受信するクラス.
/// </summary>
public class RankingClient
{
    private readonly PlayFabClient _client;
    private UserOption? _userOption;
    private readonly string _titleId;

    /// <summary>
    /// コンストラクタ.
    /// モックを使用する場合はclientに注入する.
    /// </summary>
    /// <param name="titleId"></param>
    /// <param name="client"></param>
    public RankingClient(string titleId, PlayFabClient? client = null)
    {
        _titleId = titleId;
        _client = client ?? new PlayFabClient();
    }

    /// <summary>
    /// PlayFabにログインする.
    /// 必ず最初に呼び出すこと.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async ValueTask LoginAsync(CancellationToken cancellationToken = default)
    {
        var loginRequest = new LoginWithCustomIdRequest(_titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        _userOption ??= await _client.Authentication.LoginAndGetUserOptionAsync(loginRequest, cancellationToken);
    }

    /// <summary>
    /// ランキングデータを送信する.
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="score"></param>
    /// <param name="statisticsName"></param>
    /// <param name="cancellationToken"></param>
    public async ValueTask SendAsync(
        string playerName,
        int score,
        string statisticsName = "HighScore",
        CancellationToken cancellationToken = default)
    {
        var nameRequest = new UpdateUserTitleDisplayNameRequest(playerName);
        await _client.AccountManagement.UpdateUserTitleDisplayNameAsync(nameRequest, _userOption!, cancellationToken);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = statisticsName,
                Value = score
            }
        };

        await _client.PlayerDataManagement.UpdatePlayerStatisticsAsync(
            new UpdatePlayerStatisticsRequest(statistics),
            _userOption!,
            cancellationToken
            );
    }

    /// <summary>
    /// ランキングデータをロードする.
    /// </summary>
    /// <param name="statisticName"></param>
    /// <param name="maxResultsCount"></param>
    /// <param name="cancellationToken"></param>
    public async ValueTask<(string playerName, int score)[]> LoadAsync(
        string statisticName = "HighScore",
        uint maxResultsCount = 10,
        CancellationToken cancellationToken = default)
    {
        var leaderboardRequest = new GetLeaderboardRequest(0, statisticName, maxResultsCount);
        var leaderboardResponse = await _client.PlayerDataManagement.GetLeaderboardAsync(
            leaderboardRequest,
            _userOption!,
            cancellationToken
            );
        var results = new (string, int score)[leaderboardResponse.Result.Leaderboard.Length];
        for (int i = 0; i < leaderboardResponse.Result.Leaderboard.Length; i++)
        {
            var entry = leaderboardResponse.Result.Leaderboard[i];
            results[i] = (entry.DisplayName, entry.StatValue);
        }
        return results;
    }
}