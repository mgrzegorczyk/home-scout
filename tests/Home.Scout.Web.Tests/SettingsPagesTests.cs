using System.Net;
using System.Text.RegularExpressions;

namespace Home.Scout.Web.Tests;

public class SettingsPagesTests : IClassFixture<ScoutWebApplicationFactory>
{
    private const string CategoryNav = "aria-label=\"Settings categories\"";

    // The Channels menu link, marked active, in any attribute order.
    private static readonly Regex ActiveChannelsLink = new(
        @"<a(?=[^>]*class=""nav-link active"")(?=[^>]*aria-current=""page"")(?=[^>]*href=""/settings/channels"")[^>]*>\s*<i[^>]*></i>\s*Channels");

    private static readonly Regex Hrefs = new(@"href=""([^""]*)""");

    private readonly ScoutWebApplicationFactory _factory;

    public SettingsPagesTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/settings/channels")]
    [InlineData("/settings/channels/create")]
    [InlineData("/settings/channels/edit/1")]
    [InlineData("/settings/channels/delete/1")]
    public async Task ChannelPages_RenderInsideSettingsPanel_WithChannelsActive(string url)
    {
        var html = await _factory.GetHtmlAsync(url);

        Assert.Contains(CategoryNav, html);
        Assert.Matches(ActiveChannelsLink, html);
    }

    [Theory]
    [InlineData("/settings/channels")]
    [InlineData("/settings/channels/create")]
    [InlineData("/settings/channels/edit/1")]
    [InlineData("/settings/channels/delete/1")]
    [InlineData("/News")]
    public async Task SettingsLinks_AreLowercase_AndNoOldRoutesRemain(string url)
    {
        var html = await _factory.GetHtmlAsync(url);

        var settingsHrefs = Hrefs.Matches(html)
            .Select(m => m.Groups[1].Value)
            .Where(h => h.Contains("/settings", StringComparison.OrdinalIgnoreCase));
        Assert.All(settingsHrefs, h => Assert.Equal(h.ToLowerInvariant(), h));
        Assert.DoesNotContain("href=\"/RssChannels", html, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("/settings/channels/create")]
    [InlineData("/settings/channels/edit/1")]
    public async Task FormPages_StillRenderValidationScripts(string url)
    {
        var html = await _factory.GetHtmlAsync(url);

        Assert.Contains("jquery.validate.min.js", html);
        Assert.Contains("jquery.validate.unobtrusive.min.js", html);
    }

    [Theory]
    [InlineData("/RssChannels")]
    [InlineData("/RssChannels/Create")]
    [InlineData("/RssChannels/Edit/1")]
    [InlineData("/RssChannels/Delete/1")]
    public async Task OldChannelRoutes_ReturnNotFound(string url)
    {
        var response = await _factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task News_DoesNotRenderSettingsPanel_AndEmptyStateLinksToSettings()
    {
        var html = await _factory.GetHtmlAsync("/News");

        Assert.DoesNotContain(CategoryNav, html);
        Assert.Contains("href=\"/settings/channels\"", html);
    }
}
