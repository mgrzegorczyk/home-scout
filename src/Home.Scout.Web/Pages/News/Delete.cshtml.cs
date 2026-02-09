using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.News;

public class DeleteModel : PageModel
{
    private readonly INewsItemRepository _newsItemRepository;

    public NewsItem? NewsItem { get; set; }

    public DeleteModel(INewsItemRepository newsItemRepository)
    {
        _newsItemRepository = newsItemRepository;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        NewsItem = await _newsItemRepository.GetByIdAsync(id);
        if (NewsItem == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = await _newsItemRepository.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return RedirectToPage("Index");
    }
}

