using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Home.Scout.Web.Services;

/// <summary>
/// Serwis do pobierania i parsowania kanałów RSS
/// </summary>
public class RssFeedService : IRssFeedService
{
    private readonly INewsItemRepository _newsItemRepository;
    private readonly ILogger<RssFeedService> _logger;
    private readonly HttpClient _httpClient;

    public RssFeedService(
        INewsItemRepository newsItemRepository, 
        ILogger<RssFeedService> logger,
        HttpClient httpClient)
    {
        _newsItemRepository = newsItemRepository;
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Pobiera feed z URL i zapisuje nowe artykuły do bazy danych
    /// </summary>
    public async Task<List<NewsItem>> FetchAndSaveNewsAsync(RssChannel channel)
    {
        var newItems = new List<NewsItem>();

        try
        {
            var feedItems = await FetchFeedAsync(channel.Url);
            var existingNews = await _newsItemRepository.GetByChannelIdAsync(channel.Id);
            var existingTitles = existingNews.Select(n => n.Title).ToHashSet();

            foreach (var item in feedItems)
            {
                // Omijamy duplikaty
                if (existingTitles.Contains(item.Title.Text))
                    continue;

                var newsItem = new NewsItem
                {
                    RssChannelId = channel.Id,
                    Title = item.Title.Text,
                    Description = ExtractDescription(item),
                    Link = item.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? string.Empty,
                    PublishedAt = item.PublishDate.DateTime,
                    CreatedAt = DateTime.UtcNow
                };

                var id = await _newsItemRepository.AddAsync(newsItem);
                newsItem.Id = id;
                newItems.Add(newsItem);
            }

            _logger.LogInformation("Pobrano {Count} nowych newsów z kanału {ChannelName}", 
                newItems.Count, channel.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania feedu z kanału {ChannelName}: {Message}", 
                channel.Name, ex.Message);
        }

        return newItems;
    }

    /// <summary>
    /// Pobiera feed z podanego URL
    /// </summary>
    public async Task<List<SyndicationItem>> FetchFeedAsync(string feedUrl)
    {
        var items = new List<SyndicationItem>();

        try
        {
            var content = await _httpClient.GetStringAsync(feedUrl);

            using var reader = XmlReader.Create(new StringReader(content));
            var feed = SyndicationFeed.Load(reader);
            items = feed.Items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania feedu z URL {FeedUrl}: {Message}", 
                feedUrl, ex.Message);
        }

        return items;
    }

    /// <summary>
    /// Ekstrahuje opis z elementu syndykacji
    /// </summary>
    private static string ExtractDescription(SyndicationItem item)
    {
        // Spróbuj pobrać z Summary
        if (!string.IsNullOrEmpty(item.Summary?.Text))
        {
            var text = item.Summary.Text;
            // Usuń tagi HTML
            text = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]*>", string.Empty);
            return text.Length > 200 ? text[..200] + "..." : text;
        }

        // Spróbuj pobrać z Content
        var content = item.Content as TextSyndicationContent;
        if (content != null && !string.IsNullOrEmpty(content.Text))
        {
            var text = content.Text;
            text = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]*>", string.Empty);
            return text.Length > 200 ? text[..200] + "..." : text;
        }

        return string.Empty;
    }
}


