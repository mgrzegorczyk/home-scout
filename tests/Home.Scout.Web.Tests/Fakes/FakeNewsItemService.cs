using Home.Scout.Application.News;
using Home.Scout.Application.News.Dto;

namespace Home.Scout.Web.Tests.Fakes;

public class FakeNewsItemService : INewsItemService
{
    public Task<IReadOnlyList<NewsItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<NewsItemDto>>([]);

    public Task<IReadOnlyList<NewsItemDto>> GetByChannelIdAsync(int channelId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<NewsItemDto>>([]);

    public Task<NewsItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult<NewsItemDto?>(null);

    public Task<int> CreateAsync(CreateNewsItemDto dto, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<bool> UpdateAsync(int id, UpdateNewsItemDto dto, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<bool> DeleteByChannelIdAsync(int channelId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task BulkInsertIgnoreDuplicatesAsync(IEnumerable<CreateNewsItemDto> dtos, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
