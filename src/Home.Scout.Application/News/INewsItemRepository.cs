using Home.Scout.Domain.Models;

namespace Home.Scout.Application.News;

public interface INewsItemRepository
{
    Task<IReadOnlyList<NewsItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NewsItem>> GetByChannelIdAsync(int channelId, CancellationToken cancellationToken = default);
    Task<NewsItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> AddAsync(NewsItem entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(NewsItem entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteByChannelIdAsync(int channelId, CancellationToken cancellationToken = default);

    Task BulkInsertIgnoreDuplicatesAsync(
        IEnumerable<NewsItem> entities,
        CancellationToken cancellationToken = default);
}
