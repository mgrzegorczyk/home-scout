using System.Net;

namespace Home.Scout.Web.Tests;

public class SettingsEntryPageTests : IClassFixture<ScoutWebApplicationFactory>
{
    private readonly ScoutWebApplicationFactory _factory;

    public SettingsEntryPageTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Settings_RedirectsToFirstCategory()
    {
        var response = await _factory.CreateNonRedirectingClient().GetAsync("/settings");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/settings/channels", response.Headers.Location?.OriginalString);
    }
}
