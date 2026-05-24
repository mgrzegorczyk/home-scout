using Home.Scout.Application.News;
using Home.Scout.Application.RssChannels;
using Home.Scout.Application.News.Dto;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.News;

public class IndexModel : PageModel
{
    private readonly INewsItemService _newsItemService;
    private readonly IChannelService _channelService;
    private readonly Logger<IndexModel> _logger;

    public List<NewsItemDto> NewsItems { get; private set; } = new();
    public Dictionary<int, string> ChannelNames { get; private set; } = new();

    public IndexModel(
        INewsItemService newsItemService,
        IChannelService channelService,
        Logger<IndexModel> logger)
    {
        _newsItemService = newsItemService;
        _channelService = channelService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var newsTask = _newsItemService.GetAllAsync(cancellationToken);
        var channelsTask = _channelService.GetAllAsync(cancellationToken);

        try
        {
            await Task.WhenAll(newsTask, channelsTask);

            var news = await newsTask;
            var channels = await channelsTask;

            NewsItems = news.ToList();

            ChannelNames = channels.ToDictionary(
                x => x.Id,
                x => x.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load page data.");
            throw;
        }
    }
}