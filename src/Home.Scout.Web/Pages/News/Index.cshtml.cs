using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.News;

public class IndexModel : PageModel
{
    private readonly INewsItemRepository _newsItemRepository;
    private readonly IRssChannelRepository _channelRepository;

    public List<NewsItem>? NewsItems { get; set; }
    public Dictionary<int, string> ChannelNames { get; set; } = new();

    public IndexModel(
        INewsItemRepository newsItemRepository,
        IRssChannelRepository channelRepository)
    {
        _newsItemRepository = newsItemRepository;
        _channelRepository = channelRepository;
    }

    public async Task OnGetAsync()
    {
        NewsItems = await _newsItemRepository.GetAllAsync();
        
        var channels = await _channelRepository.GetAllAsync();
        ChannelNames = channels.ToDictionary(c => c.Id, c => c.Name);
    }
}



