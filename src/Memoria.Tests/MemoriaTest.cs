namespace Memoria.Tests;

public class MemoriaTest
{
    private const string TitleId = "HogeTitleId";
    private const string SessionTicket = "FAKE_SESSION_TICKET";

    [Test]
    public async Task LoginWithCustomIdTest()
    {
        var handler = new MoqPlayFabHandler();
        var fakeResponse = new LoginWithCustomIdResponse
        {
            Result = new LoginResult
            {
                SessionTicket = SessionTicket,
                NewlyCreated = true,
            },
        };
        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var request = new LoginWithCustomIdRequest(TitleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        var response = await client.Authentication.LoginWithCustomIdAsync(request);

        Assert.That(response.Result.SessionTicket, Is.EqualTo(SessionTicket));
        Assert.That(response.Result.NewlyCreated, Is.True);
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/LoginWithCustomID"));
        Assert.That(handler.LastRequestBody, Contains.Substring(TitleId));
    }

    [Test]
    public async Task LoginAndGetUserOptionTest()
    {
        var handler = new MoqPlayFabHandler();
        var fakeResponse = new LoginWithCustomIdResponse()
        {
            Result = new LoginResult
            {
                SessionTicket = SessionTicket,
                NewlyCreated = true,
            },
        };

        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var request = new LoginWithCustomIdRequest(TitleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };
        var response = await client.Authentication.LoginAndGetUserOptionAsync(request);
        Assert.That(response.LoginResult.SessionTicket, Is.EqualTo(SessionTicket));
        Assert.That(response.LoginResult.NewlyCreated, Is.True);
        Assert.That(response.TitleId, Is.EqualTo(TitleId));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/LoginWithCustomID"));
    }

    [Test]
    public async Task UpdatePlayerStatisticsTest()
    {
        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UpdatePlayerStatisticsResponse();
        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = "HighScore",
                Value = "1000",
            },
        };
        var request = new UpdatePlayerStatisticsRequest(statistics);
        var response = await client.PlayerDataManagement.UpdatePlayerStatisticsAsync(request, TitleId, SessionTicket);

        Assert.That(response, Is.EqualTo(fakeResponse));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/UpdatePlayerStatistics"));
        Assert.That(handler.LastRequestBody, Contains.Substring("HighScore"));
        Assert.That(handler.LastRequestBody, Contains.Substring("1000"));
    }

    [Test]
    public async Task UpdatePlayerStatistics_WithUserOptionTest()
    {
        var userOption = new UserOption(
            new LoginResult
            {
                SessionTicket = SessionTicket,
                NewlyCreated = true
            }, TitleId: TitleId);

        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UpdatePlayerStatisticsResponse();

        handler.ResponseData = fakeResponse;

        var client = new PlayFabClient(handler);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = "HighScore",
                Value = "2000",
            },
        };
        var request = new UpdatePlayerStatisticsRequest(statistics);
        var response = await client.PlayerDataManagement.UpdatePlayerStatisticsAsync(request, userOption);

        Assert.That(response, Is.EqualTo(fakeResponse));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/UpdatePlayerStatistics"));
        Assert.That(handler.LastRequestBody, Contains.Substring("HighScore"));
        Assert.That(handler.LastRequestBody, Contains.Substring("2000"));
    }

    [Test]
    public async Task UpdateUserTitleDisplayNameTest()
    {
        const string newDisplayName = "NewDisplayName";

        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UpdateUserTitleDisplayNameResponse
        {
            Result = new UpdateUserTitleDisplayNameResult
            {
                DisplayName = newDisplayName,
            },
        };

        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var request = new UpdateUserTitleDisplayNameRequest(newDisplayName);
        var response = await client.AccountManagement.UpdateUserTitleDisplayNameAsync(request, TitleId, SessionTicket);

        Assert.That(response, Is.EqualTo(fakeResponse));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/UpdateUserTitleDisplayName"));
        Assert.That(handler.LastRequestBody, Contains.Substring(newDisplayName));
    }

    [Test]
    public async Task UpdateUserTitleDisplayNameAsync_WithUserOptionTest()
    {
        const string newDisplayName = "AnotherNewDisplayName";

        var userOption = new UserOption(
            new LoginResult
            {
                SessionTicket = SessionTicket,
                NewlyCreated = true
            }, TitleId: TitleId);
        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UpdateUserTitleDisplayNameResponse
        {
            Result = new UpdateUserTitleDisplayNameResult
            {
                DisplayName = newDisplayName,
            },
        };

        handler.ResponseData = fakeResponse;

        var client = new PlayFabClient(handler);
        var request = new UpdateUserTitleDisplayNameRequest(newDisplayName);
        var response = await client.AccountManagement.UpdateUserTitleDisplayNameAsync(request, userOption);

        Assert.That(response, Is.EqualTo(fakeResponse));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/UpdateUserTitleDisplayName"));
        Assert.That(handler.LastRequestBody, Contains.Substring(newDisplayName));
    }

    [Test]
    public async Task SendRankingDataTest()
    {
        const string playerName = "PlayerOne";
        const string statisticName = "HighScore";
        const int scoreValue = 1500;

        var handler = new MoqPlayFabHandler();
        handler.ResponseData = new LoginWithCustomIdResponse()
        {
            Result = new LoginResult
            {
                SessionTicket = SessionTicket,
                NewlyCreated = true,
            },
        };

        var data = new RankingData<int>
        {
            PlayerName = playerName,
            StatisticName = statisticName,
            Score = scoreValue
        };

        var client = new PlayFabClient(handler);
        var register = new RankingRegister(TitleId, client);
        await register.SendAsync(data);

        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequestBody, Contains.Substring(statisticName));
        Assert.That(handler.LastRequestBody, Contains.Substring(scoreValue.ToString()));

        var data2 = new RankingData<int>
        {
            PlayerName = "PlayerTwo",
            StatisticName = statisticName,
            Score = scoreValue + 500
        };

        await register.SendAsync(data2);

        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequestBody, Contains.Substring(statisticName));
        Assert.That(handler.LastRequestBody, Contains.Substring((scoreValue + 500).ToString()));
    }
}