using Home.Scout.Application.RssChannels;
using Home.Scout.Application.Channels.Dto;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.RssChannels;

public class IndexModel : PageModel
{
    private readonly IChannelService _channelService;

    public IReadOnlyList<RssChannelDto> Channels { get; private set; }
        = new List<RssChannelDto>();

    public IndexModel(IChannelService channelService)
    {
        _channelService = channelService;
    }

    public async Task OnGetAsync()
    {
        Channels = await _channelService.GetAllAsync();
    }
}