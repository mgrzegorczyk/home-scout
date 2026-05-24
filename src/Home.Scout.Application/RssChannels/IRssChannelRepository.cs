using Home.Scout.Domain.Models;

namespace Home.Scout.Application.RssChannels;

public interface IRssChannelRepository
{
    Task<List<RssChannel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RssChannel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(RssChannel rssChannel, CancellationToken cancellationToken = default);
    Task UpdateAsync(RssChannel rssChannel, CancellationToken cancellationToken = default);
    Task DeleteAsync(RssChannel rssChannel, CancellationToken cancellationToken = default);
}