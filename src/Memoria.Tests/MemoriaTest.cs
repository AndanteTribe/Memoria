namespace Memoria.Tests;

public class MemoriaTest
{
    [Test]
    public async Task LoginWithCustomIdTest()
    {
        const string titleId = "HogeTitleId";
        const string sessionTicket = "FAKE_SESSION_TICKET";

        var handler = new MoqPlayFabHandler();
        var fakeResponse = new LoginWithCustomIdResponse
        {
            Result = new LoginResult
            {
                SessionTicket = sessionTicket,
                NewlyCreated = true,
            },
        };
        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var request = new LoginWithCustomIdRequest(titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };

        var response = await client.Authentication.LoginWithCustomIdAsync(request);
        
        Assert.That(response.Result.SessionTicket, Is.EqualTo(sessionTicket));
        Assert.That(response.Result.NewlyCreated, Is.True);
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/LoginWithCustomID"));
        Assert.That(handler.LastRequestBody, Contains.Substring(titleId));
    }

    [Test]
    public async Task LoginAndGetUserOptionTest()
    {
        const string titleId = "HogeTitleId";
        const string sessionTicket = "FAKE_SESSION_TICKET";

        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UserOption(
            new LoginResult {
                SessionTicket = sessionTicket,
                NewlyCreated = true
            }, TitleId: titleId);

        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var request = new LoginWithCustomIdRequest(titleId)
        {
            CustomId = Guid.NewGuid(),
            CreateAccount = true,
        };
        var response = await client.Authentication.LoginAndGetUserOptionAsync(request);

        Assert.That(response.LoginResult.SessionTicket, Is.EqualTo(sessionTicket));
        Assert.That(response.LoginResult.NewlyCreated, Is.True);
        Assert.That(response.TitleId, Is.EqualTo(titleId));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/LoginWithCustomID"));
    }

    [Test]
    public async Task UpdatePlayerStatisticsTest()
    {
        const string titleId = "HogeTitleId";
        const string sessionTicket = "FAKE_SESSION_TICKET";

        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UpdatePlayerStatisticsResponse();
        handler.ResponseData = fakeResponse;
        var client = new PlayFabClient(handler);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = "HighScore",
                Value = 1000,
            },
        };
        var request = new UpdatePlayerStatisticsRequest(statistics);
        var response = await client.PlayerDataManagement.UpdatePlayerStatisticsAsync(request, titleId, sessionTicket);

        Assert.That(response, Is.EqualTo(fakeResponse));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/UpdatePlayerStatistics"));
        Assert.That(handler.LastRequestBody, Contains.Substring("HighScore"));
        Assert.That(handler.LastRequestBody, Contains.Substring("1000"));
    }

    [Test]
    public async Task UpdatePlayerStatistics_WithUserOptionTest()
    {
        const string titleId = "HogeTitleId";
        const string sessionTicket = "FAKE_SESSION_TICKET";

        var userOption = new UserOption(
            new LoginResult
            {
                SessionTicket = sessionTicket,
                NewlyCreated = true
            }, TitleId: titleId);

        var handler = new MoqPlayFabHandler();
        var fakeResponse = new UpdatePlayerStatisticsResponse();

        handler.ResponseData = fakeResponse;

        var client = new PlayFabClient(handler);
        var statistics = new[]
        {
            new StatisticUpdate
            {
                StatisticName = "HighScore",
                Value = 2000,
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
        const string titleId = "HogeTitleId";
        const string sessionTicket = "FAKE_SESSION_TICKET";
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
        var response = await client.AccountManagement.UpdateUserTitleDisplayNameAsync(request, titleId, sessionTicket);

        Assert.That(response, Is.EqualTo(fakeResponse));
        Assert.That(handler.LastRequest, Is.Not.Null);
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Contains.Substring("/Client/UpdateUserTitleDisplayName"));
        Assert.That(handler.LastRequestBody, Contains.Substring(newDisplayName));
    }

    [Test]
    public async Task UpdateUserTitleDisplayNameAsync_WithUserOptionTest()
    {
        const string titleId = "HogeTitleId";
        const string sessionTicket = "FAKE_SESSION_TICKET";
        const string newDisplayName = "AnotherNewDisplayName";

        var userOption = new UserOption(
            new LoginResult
            {
                SessionTicket = sessionTicket,
                NewlyCreated = true
            }, TitleId: titleId);
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
}