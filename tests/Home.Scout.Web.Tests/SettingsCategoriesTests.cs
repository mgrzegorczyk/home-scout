using Home.Scout.Web.Settings;

namespace Home.Scout.Web.Tests;

public class SettingsCategoriesTests
{
    [Fact]
    public void All_IsNonEmpty_WithUniqueKeys()
    {
        Assert.NotEmpty(SettingsCategories.All);
        Assert.Equal(
            SettingsCategories.All.Count,
            SettingsCategories.All.Select(c => c.Key).Distinct().Count());
    }

    [Fact]
    public void All_StartsWithChannels()
    {
        var first = SettingsCategories.All[0];

        Assert.Equal("channels", first.Key);
        Assert.Equal("/RssChannels/Index", first.EntryPage);
    }

    [Theory]
    [InlineData("/RssChannels")]
    [InlineData("/RssChannels/Index")]
    [InlineData("/RssChannels/Edit")]
    [InlineData("/RssChannels/Create")]
    [InlineData("/RssChannels/Delete")]
    public void FindForPage_MapsChannelPagesToChannels(string pagePath)
    {
        Assert.Equal("channels", SettingsCategories.FindForPage(pagePath)?.Key);
    }

    [Theory]
    [InlineData("/News/Index")]
    [InlineData("/RssChannelsArchive")]
    [InlineData("/RssChannelsArchive/Index")]
    [InlineData("/Settings/Index")]
    [InlineData("")]
    [InlineData(null)]
    public void FindForPage_ReturnsNullForOtherPages(string? pagePath)
    {
        Assert.Null(SettingsCategories.FindForPage(pagePath));
    }
}
