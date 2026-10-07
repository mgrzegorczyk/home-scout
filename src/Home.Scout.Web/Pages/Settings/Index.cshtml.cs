using Home.Scout.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.Settings;

/// <summary>
/// Entry point of the settings panel: redirects to the first registered category.
/// </summary>
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (SettingsCategories.All.Count == 0)
            return NotFound();

        return RedirectToPage(SettingsCategories.All[0].EntryPage);
    }
}
