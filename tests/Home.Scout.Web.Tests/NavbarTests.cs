using System.Text.RegularExpressions;

namespace Home.Scout.Web.Tests;

public class NavbarTests : IClassFixture<ScoutWebApplicationFactory>
{
    // The navbar gear link, in any attribute order; captures the opening tag and the icon class.
    private static readonly Regex GearLink = new(
        @"<a(?=[^>]*href=""/settings"")(?=[^>]*aria-label=""Settings"")(?=[^>]*title=""Settings"")[^>]*>\s*<i[^>]*class=""bi (?<icon>bi-gear(-fill)?)""");

    private readonly ScoutWebApplicationFactory _factory;

    public NavbarTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task News_HasInactiveGear_AndNoChannelsNavLink()
    {
        var html = await _factory.GetHtmlAsync("/News");

        var gear = GearLink.Match(html);
        Assert.True(gear.Success, "Gear link to /settings not found.");
        Assert.DoesNotContain("aria-current", gear.Value);
        Assert.Equal("bi-gear", gear.Groups["icon"].Value);
        Assert.DoesNotContain(">Channels</a>", html);
    }

    [Theory]
    [InlineData("/settings/channels")]
    [InlineData("/settings/channels/create")]
    [InlineData("/settings/channels/edit/1")]
    [InlineData("/settings/channels/delete/1")]
    public async Task SettingsPages_MarkGearAsCurrent(string url)
    {
        var html = await _factory.GetHtmlAsync(url);

        var gear = GearLink.Match(html);
        Assert.True(gear.Success, "Gear link to /settings not found.");
        Assert.Contains("aria-current=\"page\"", gear.Value);
        Assert.Contains("active", gear.Value);
        Assert.Equal("bi-gear", gear.Groups["icon"].Value);
    }

    [Fact]
    public async Task SiteCss_ReservesScrollbarGutter_SoTheNavbarDoesNotShift()
    {
        var css = await _factory.GetHtmlAsync("/css/site.css");

        Assert.Matches(new Regex(@"html\s*\{[^}]*scrollbar-gutter:\s*stable;"), css);
    }

    [Fact]
    public async Task Gear_HasVisibleLabelOnlyBelowSm()
    {
        var html = await _factory.GetHtmlAsync("/News");

        Assert.Matches(new Regex(@"<span[^>]*class=""d-sm-none ms-1""[^>]*>Settings</span>"), html);
    }
}
