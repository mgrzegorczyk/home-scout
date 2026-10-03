namespace Home.Scout.Web.Settings;

/// <summary>
/// One category of the settings panel. <paramref name="PagePrefix"/> is the Razor page path
/// (e.g. "/RssChannels") under which every page of the category lives.
/// </summary>
public sealed record SettingsCategory(
    string Key,
    string Title,
    string Icon,
    string EntryPage,
    string PagePrefix);

/// <summary>
/// Ordered registry of settings categories. The first entry is where <c>/Settings</c> lands.
/// Add a category by adding a line here plus its pages.
/// </summary>
public static class SettingsCategories
{
    public static IReadOnlyList<SettingsCategory> All { get; } =
    [
        new("channels", "Channels", "bi-rss", "/RssChannels/Index", "/RssChannels")
    ];

    /// <summary>
    /// Returns the category a Razor page path (e.g. <c>/RssChannels/Edit</c>) belongs to, or null.
    /// </summary>
    public static SettingsCategory? FindForPage(string? pagePath)
    {
        if (string.IsNullOrEmpty(pagePath))
            return null;

        return All.FirstOrDefault(category =>
            string.Equals(pagePath, category.PagePrefix, StringComparison.OrdinalIgnoreCase)
            || pagePath.StartsWith(category.PagePrefix + "/", StringComparison.OrdinalIgnoreCase));
    }
}
