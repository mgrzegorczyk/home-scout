using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.Channels;

public class DeleteModel : PageModel
{
    private readonly IRssChannelRepository _channelRepository;
    private readonly INewsItemRepository _newsItemRepository;

    public RssChannel? Channel { get; set; }

    public DeleteModel(
        IRssChannelRepository channelRepository,
        INewsItemRepository newsItemRepository)
    {
        _channelRepository = channelRepository;
        _newsItemRepository = newsItemRepository;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Channel = await _channelRepository.GetByIdAsync(id);
        if (Channel == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Najpierw usuń wszystkie artykuły z tego kanału
        await _newsItemRepository.DeleteByChannelIdAsync(id);

        // Potem usuń kanał
        var result = await _channelRepository.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return RedirectToPage("Index");
    }
}

