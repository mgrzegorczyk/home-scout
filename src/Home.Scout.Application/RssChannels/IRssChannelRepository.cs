
using Home.Scout.Domain.Models;

namespace Home.Scout.Application.RssChannels;

public interface IRssChannelRepository
{
    Task<List<RssChannel>> GetAllAsync();
    Task<RssChannel?> GetByIdAsync(int id);
    Task AddAsync(RssChannel rssChannel);
    Task UpdateAsync(RssChannel rssChannel);
    Task DeleteAsync(RssChannel rssChannel);
}


