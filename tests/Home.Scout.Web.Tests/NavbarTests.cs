using System.Net;
using System.Text.RegularExpressions;

namespace Home.Scout.Web.Tests;

public class NavbarTests : IClassFixture<ScoutWebApplicationFactory>
{
    // The navbar gear link, in any attribute order; captures the opening tag and the icon class.
    private static readonly Regex GearLink = new(
        @"<a(?=[^>]*href=""/Settings"")(?=[^>]*aria-label=""Settings"")(?=[^>]*title=""Settings"")[^>]*>\s*<i[^>]*class=""bi (?<icon>bi-gear(-fill)?)""");

    private readonly ScoutWebApplicationFactory _factory;

    public NavbarTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task News_HasGearLink_AndNoChannelsNavLink()
    {
        var html = await GetOkHtmlAsync("/News");

        var gear = GearLink.Match(html);
        Assert.True(gear.Success, "Gear link to /Settings not found.");
        Assert.DoesNotContain("aria-current", gear.Value);
        Assert.Equal("bi-gear", gear.Groups["icon"].Value);
        Assert.DoesNotContain(">Channels</a>", html);
    }

    [Theory]
    [InlineData("/RssChannels")]
    [InlineData("/RssChannels/Create")]
    [InlineData("/RssChannels/Edit/1")]
    public async Task SettingsPages_MarkGearAsCurrent(string url)
    {
        var html = await GetOkHtmlAsync(url);

        var gear = GearLink.Match(html);
        Assert.True(gear.Success, "Gear link to /Settings not found.");
        Assert.Contains("aria-current=\"page\"", gear.Value);
        Assert.Contains("active", gear.Value);
        Assert.Equal("bi-gear-fill", gear.Groups["icon"].Value);
    }

    private async Task<string> GetOkHtmlAsync(string url)
    {
        var response = await _factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadAsStringAsync();
    }
}
