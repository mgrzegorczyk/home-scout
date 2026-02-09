using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Home.Scout.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.Channels;

public class FetchModel : PageModel
{
    private readonly IRssFeedService _rssFeedService;
    private readonly IRssChannelRepository _channelRepository;
    private readonly ILogger<FetchModel> _logger;

    public RssChannel? Channel { get; set; }
    public List<NewsItem> FetchedItems { get; set; } = new();
    public string? Message { get; set; }

    public FetchModel(
        IRssFeedService rssFeedService,
        IRssChannelRepository channelRepository,
        ILogger<FetchModel> logger)
    {
        _rssFeedService = rssFeedService;
        _channelRepository = channelRepository;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Channel = await _channelRepository.GetByIdAsync(id);
        if (Channel == null)
        {
            return NotFound();
        }

        try
        {
            FetchedItems = await _rssFeedService.FetchAndSaveNewsAsync(Channel);
            
            if (FetchedItems.Count == 0)
            {
                Message = "Nie znaleziono nowych artykułów lub wszystkie artykuły już istnieją w bazie.";
            }
            else
            {
                Message = $"Pomyślnie pobrano {FetchedItems.Count} nowych artykułów.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania kanału {ChannelId}", id);
            Message = $"Błąd podczas pobierania: {ex.Message}";
        }

        return Page();
    }
}


