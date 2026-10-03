using System.Net;

namespace Home.Scout.Web.Tests;

public class SettingsPageTests : IClassFixture<ScoutWebApplicationFactory>
{
    private readonly ScoutWebApplicationFactory _factory;

    public SettingsPageTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Settings_RedirectsToFirstCategory()
    {
        var client = _factory.CreateNonRedirectingClient();

        var response = await client.GetAsync("/Settings");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/RssChannels", response.Headers.Location?.OriginalString);
    }
}
