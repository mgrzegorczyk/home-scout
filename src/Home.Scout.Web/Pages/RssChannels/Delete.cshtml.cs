using Home.Scout.Application.RssChannels;
using Home.Scout.Application.Channels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.RssChannels;

public class DeleteModel : PageModel
{
    private readonly IChannelService _channelService;

    public RssChannelDto? Channel { get; set; }

    public DeleteModel(IChannelService channelService)
    {
        _channelService = channelService;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Channel = await _channelService.GetByIdAsync(id);

        if (Channel == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _channelService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}