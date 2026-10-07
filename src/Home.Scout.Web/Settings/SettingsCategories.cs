namespace Home.Scout.Web.Settings;

/// <summary>
/// One category of the settings panel. <paramref name="PagePrefix"/> is a Razor page path
/// (e.g. "/Settings/Channels"), not a URL.
/// </summary>
public sealed record SettingsCategory(
    string Key,
    string Title,
    string Icon,
    string EntryPage,
    string PagePrefix);

/// <summary>
/// Ordered registry of settings categories. The first entry is where <c>/settings</c> lands.
/// Add a category by adding a line here plus its pages under <c>Pages/Settings/{Category}/</c>.
/// </summary>
public static class SettingsCategories
{
    public static IReadOnlyList<SettingsCategory> All { get; } =
    [
        new("channels", "Channels", "bi-rss", "/Settings/Channels/Index", "/Settings/Channels")
    ];

    /// <summary>
    /// Returns the category a Razor page path (e.g. <c>/Settings/Channels/Edit</c>) belongs to, or null.
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
