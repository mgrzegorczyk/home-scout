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
        var html = await _factory.GetHtmlAsync("/News");

        Assert.Contains("navbar-brand", html);
    }
}
