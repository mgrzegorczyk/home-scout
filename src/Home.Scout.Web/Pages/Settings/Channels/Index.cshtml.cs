using Home.Scout.Application.RssChannels;
using Home.Scout.Application.Channels.Dto;
using Home.Scout.Application.Rss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.Settings.Channels;

public class IndexModel : PageModel
{
    private readonly IChannelService _channelService;
    private readonly IRssSyncService _rssSyncService;

    public IReadOnlyList<RssChannelDto> Channels { get; private set; }
        = new List<RssChannelDto>();

    [TempData]
    public string? StatusMessage { get; set; }

    public IndexModel(IChannelService channelService, IRssSyncService rssSyncService)
    {
        _channelService = channelService;
        _rssSyncService = rssSyncService;
    }

    public async Task OnGetAsync()
    {
        Channels = await _channelService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostFetchNowAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var count = await _rssSyncService.SyncChannelAsync(id, cancellationToken);
            StatusMessage = $"Fetched {count} item(s).";
        }
        catch (KeyNotFoundException)
        {
            StatusMessage = "Channel not found.";
        }
        catch (Exception)
        {
            StatusMessage = "Failed to fetch the RSS feed for this channel.";
        }

        return RedirectToPage();
    }
}
