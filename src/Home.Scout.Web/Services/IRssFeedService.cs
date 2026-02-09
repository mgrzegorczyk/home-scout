using Home.Scout.Web.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Home.Scout.Web.Services;

/// <summary>
/// Interfejs serwisu RSS
/// </summary>
public interface IRssFeedService
{
    Task<List<NewsItem>> FetchAndSaveNewsAsync(RssChannel channel);
    Task<List<SyndicationItem>> FetchFeedAsync(string feedUrl);
}


