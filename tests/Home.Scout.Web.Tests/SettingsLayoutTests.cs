using System.Net;
using System.Text.RegularExpressions;

namespace Home.Scout.Web.Tests;

public class SettingsLayoutTests : IClassFixture<ScoutWebApplicationFactory>
{
    private const string CategoryNav = "aria-label=\"Settings categories\"";

    // The Channels menu link, marked active, in any attribute order.
    private static readonly Regex ActiveChannelsLink = new(
        @"<a(?=[^>]*class=""nav-link active"")(?=[^>]*aria-current=""page"")(?=[^>]*href=""/RssChannels"")[^>]*>\s*<i[^>]*></i>\s*Channels");

    private readonly ScoutWebApplicationFactory _factory;

    public SettingsLayoutTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/RssChannels")]
    [InlineData("/RssChannels/Create")]
    [InlineData("/RssChannels/Edit/1")]
    [InlineData("/RssChannels/Delete/1")]
    public async Task ChannelPages_RenderInsideSettingsPanel_WithChannelsActive(string url)
    {
        var html = await GetOkHtmlAsync(url);

        Assert.Contains(CategoryNav, html);
        Assert.Matches(ActiveChannelsLink, html);
    }

    [Theory]
    [InlineData("/RssChannels/Create")]
    [InlineData("/RssChannels/Edit/1")]
    public async Task FormPages_StillRenderValidationScripts(string url)
    {
        var html = await GetOkHtmlAsync(url);

        Assert.Contains("jquery.validate.min.js", html);
        Assert.Contains("jquery.validate.unobtrusive.min.js", html);
    }

    [Fact]
    public async Task News_DoesNotRenderSettingsPanel()
    {
        var html = await GetOkHtmlAsync("/News");

        Assert.DoesNotContain(CategoryNav, html);
    }

    private async Task<string> GetOkHtmlAsync(string url)
    {
        var response = await _factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadAsStringAsync();
    }
}
