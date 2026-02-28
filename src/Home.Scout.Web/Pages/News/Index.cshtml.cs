using Home.Scout.Application.News;
using Home.Scout.Application.RssChannels;
using Home.Scout.Application.News.Dto;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.News;

public class IndexModel : PageModel
{
    private readonly INewsItemService _newsItemService;
    private readonly IChannelService _channelService;

    public List<NewsItemDto> NewsItems { get; private set; } = new();
    public Dictionary<int, string> ChannelNames { get; private set; } = new();

    public IndexModel(
        INewsItemService newsItemService,
        IChannelService channelService)
    {
        _newsItemService = newsItemService;
        _channelService = channelService;
    }

    public async Task OnGetAsync()
    {
        var newsTask = _newsItemService.GetAllAsync();
        var channelsTask = _channelService.GetAllAsync();

        await Task.WhenAll(newsTask, channelsTask);

        NewsItems = newsTask.Result.ToList();

        ChannelNames = channelsTask.Result
            .ToDictionary(x => x.Id, x => x.Name);
    }
}