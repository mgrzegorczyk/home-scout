using System.Net;

namespace Home.Scout.Web.Tests;

public class SmokeTests : IClassFixture<ScoutWebApplicationFactory>
{
    private readonly ScoutWebApplicationFactory _factory;

    public SmokeTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task News_RendersLayout()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/News");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("navbar-brand", html);
    }
}
