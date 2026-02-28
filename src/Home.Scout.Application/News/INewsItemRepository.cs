using Home.Scout.Domain.Models;

namespace Home.Scout.Application.News;

public interface INewsItemRepository
{
    Task<List<NewsItem>> GetAllAsync();
    Task<List<NewsItem>> GetByChannelIdAsync(int channelId);
    Task<NewsItem?> GetByIdAsync(int id);
    Task<int> AddAsync(NewsItem newsItem);
    Task<bool> UpdateAsync(NewsItem newsItem);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByChannelIdAsync(int channelId);
    Task BulkInsertIgnoreDuplicatesAsync(IEnumerable<NewsItem> items);
}


