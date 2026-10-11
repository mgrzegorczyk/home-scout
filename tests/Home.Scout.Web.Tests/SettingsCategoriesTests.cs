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
        Assert.Equal("/Settings/Channels/Index", first.EntryPage);
    }

    [Theory]
    [InlineData("/Settings/Channels")]
    [InlineData("/Settings/Channels/Index")]
    [InlineData("/Settings/Channels/Create")]
    [InlineData("/Settings/Channels/Edit")]
    [InlineData("/Settings/Channels/Delete")]
    public void FindForPage_MapsChannelPagesToChannels(string pagePath)
    {
        Assert.Equal("channels", SettingsCategories.FindForPage(pagePath)?.Key);
    }

    [Theory]
    [InlineData("/News/Index")]
    [InlineData("/Settings/ChannelsArchive")]
    [InlineData("/Settings/ChannelsArchive/Index")]
    [InlineData("/Settings/Index")]
    [InlineData("")]
    [InlineData(null)]
    public void FindForPage_ReturnsNullForOtherPages(string? pagePath)
    {
        Assert.Null(SettingsCategories.FindForPage(pagePath));
    }
}
