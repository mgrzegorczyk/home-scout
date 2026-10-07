using System.Text.RegularExpressions;

namespace Home.Scout.Web.Tests;

public class ChannelTableTests : IClassFixture<ScoutWebApplicationFactory>
{
    private readonly ScoutWebApplicationFactory _factory;

    public ChannelTableTests(ScoutWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("Description")]
    [InlineData("Added at")]
    public async Task SecondaryColumnHeaders_AreHiddenBelowMd(string header)
    {
        var html = await _factory.GetHtmlAsync("/settings/channels");

        Assert.Contains($"<th class=\"d-none d-md-table-cell\">{header}</th>", html);
    }

    [Fact]
    public async Task SecondaryColumnCells_AreHiddenBelowMd()
    {
        var html = await _factory.GetHtmlAsync("/settings/channels");

        // One faked channel: its Description and Added at cells are both responsive.
        Assert.Equal(2, Regex.Matches(html, @"<td class=""[^""]*\bd-none d-md-table-cell""").Count);
    }

    [Fact]
    public async Task Row_HasActionsDropdown_BelowMd()
    {
        var html = await _factory.GetHtmlAsync("/settings/channels");
        var dropdown = Section(html, "<div class=\"dropdown d-md-none\">", "</ul>");

        Assert.Matches(new Regex(@"<button(?=[^>]*data-bs-toggle=""dropdown"")(?=[^>]*aria-label=""Actions for Test Channel"")"), dropdown);
        Assert.Matches(new Regex(@"<form method=""post"" action=""/settings/channels\?(?=[^""]*handler=FetchNow)(?=[^""]*\bid=1)"), dropdown);
        Assert.Contains("__RequestVerificationToken", dropdown);
        Assert.Contains("href=\"/settings/channels/edit/1\"", dropdown);
        Assert.Contains("href=\"/settings/channels/delete/1\"", dropdown);
    }

    [Fact]
    public async Task Row_KeepsButtonGroup_FromMd()
    {
        var html = await _factory.GetHtmlAsync("/settings/channels");
        var buttons = Section(html, "<div class=\"d-none d-md-inline-flex flex-wrap gap-1\">", "</div>");

        Assert.Contains("Fetch now", buttons);
        Assert.Contains("href=\"/settings/channels/edit/1\"", buttons);
        Assert.Contains("href=\"/settings/channels/delete/1\"", buttons);
    }

    [Fact]
    public async Task Table_HasNoScrollContainer_ThatWouldClipTheDropdown()
    {
        var html = await _factory.GetHtmlAsync("/settings/channels");

        Assert.DoesNotContain("table-responsive", html);
        Assert.Contains("<div class=\"channels-table\">", html);
    }

    [Fact]
    public async Task NameAndUrl_TruncateWithFullValueInTitle()
    {
        var html = await _factory.GetHtmlAsync("/settings/channels");

        Assert.Contains("<td class=\"text-truncate\" title=\"Test Channel\">", html);
        var urlAnchor = Regex.Match(html, @"<a href=""https://example\.com/feed\.xml""[^>]*>");
        Assert.True(urlAnchor.Success, "URL anchor not found.");
        Assert.Contains("class=\"channel-url d-block text-truncate\"", urlAnchor.Value);
        Assert.Contains("title=\"https://example.com/feed.xml\"", urlAnchor.Value);
        Assert.DoesNotContain("max-width", urlAnchor.Value);
    }

    private static string Section(string html, string start, string end)
    {
        var from = html.IndexOf(start, StringComparison.Ordinal);
        Assert.True(from >= 0, $"'{start}' not found.");
        var to = html.IndexOf(end, from, StringComparison.Ordinal);
        Assert.True(to > from, $"'{end}' not found after '{start}'.");
        return html[from..(to + end.Length)];
    }
}
