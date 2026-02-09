using Home.Scout.Web.Models;

namespace Home.Scout.Web.Data;

/// <summary>
/// Interfejs dla repozytoriów kanałów RSS
/// </summary>
public interface IRssChannelRepository
{
    Task<List<RssChannel>> GetAllAsync();
    Task<RssChannel?> GetByIdAsync(int id);
    Task<int> AddAsync(RssChannel channel);
    Task<bool> UpdateAsync(RssChannel channel);
    Task<bool> DeleteAsync(int id);
}


