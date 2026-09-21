using System.ServiceModel.Syndication;
using System.Xml;
using Home.Scout.Application.Channels.Dto;
using Home.Scout.Application.News.Dto;
using Microsoft.Extensions.Logging;

namespace Home.Scout.Application.Rss;

public interface IRssFeedService
{
    Task<List<NewsItemDto>> FetchNewsAsync(RssChannelDto channel);
}

public class RssFeedService : IRssFeedService
{
    private readonly ILogger<RssFeedService> _logger;
    private readonly HttpClient _httpClient;

    public RssFeedService(
        ILogger<RssFeedService> logger,
        HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<List<NewsItemDto>> FetchNewsAsync(RssChannelDto channel)
    {
        var results = new List<NewsItemDto>();

        try
        {
            using var response = await _httpClient.GetAsync(channel.Url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = XmlReader.Create(stream);

            var feed = SyndicationFeed.Load(reader);

            foreach (var item in feed.Items)
            {
                results.Add(new NewsItemDto
                {
                    RssChannelId = channel.Id,
                    Title = item.Title?.Text ?? "",
                    Description = ExtractDescription(item),
                    Link = item.Links.FirstOrDefault()?.Uri.AbsoluteUri ?? "",
                    PublishedAt = item.PublishDate.UtcDateTime,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RSS error for channel {Channel}", channel.Name);
        }

        return results;
    }

    private static string ExtractDescription(SyndicationItem item)
    {
        var text = item.Summary?.Text ?? "";

        text = System.Text.RegularExpressions.Regex
            .Replace(text, "<[^>]*>", string.Empty);

        return text.Length > 200 ? text[..200] + "..." : text;
    }
}
