using Home.Scout.Application.News.Dto;
using Home.Scout.Domain.Models;

namespace Home.Scout.Application.News;

public interface INewsItemService
{
    Task<IReadOnlyList<NewsItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NewsItemDto>> GetByChannelIdAsync(int channelId, CancellationToken cancellationToken = default);
    Task<NewsItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateNewsItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateNewsItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteByChannelIdAsync(int channelId, CancellationToken cancellationToken = default);

    Task BulkInsertIgnoreDuplicatesAsync(
        IEnumerable<CreateNewsItemDto> dtos,
        CancellationToken cancellationToken = default);
}

public class NewsItemService : INewsItemService
{
    private readonly INewsItemRepository _repository;

    public NewsItemService(INewsItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<NewsItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<NewsItemDto>> GetByChannelIdAsync(
        int channelId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByChannelIdAsync(channelId, cancellationToken);

        return items.Select(Map).ToList();
    }

    public async Task<NewsItemDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);

        return item == null ? null : Map(item);
    }

    public async Task<int> CreateAsync(
        CreateNewsItemDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new NewsItem
        {
            RssChannelId = dto.RssChannelId,
            Title = dto.Title,
            Description = dto.Description,
            Link = dto.Link,
            PublishedAt = dto.PublishedAt,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateNewsItemDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new NewsItem
        {
            Id = id,
            RssChannelId = dto.RssChannelId,
            Title = dto.Title,
            Description = dto.Description,
            Link = dto.Link,
            PublishedAt = dto.PublishedAt
        };

        return await _repository.UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);

    public Task<bool> DeleteByChannelIdAsync(
        int channelId,
        CancellationToken cancellationToken = default)
        => _repository.DeleteByChannelIdAsync(channelId, cancellationToken);

    public async Task BulkInsertIgnoreDuplicatesAsync(
        IEnumerable<CreateNewsItemDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var entities = dtos.Select(dto => new NewsItem
        {
            RssChannelId = dto.RssChannelId,
            Title = dto.Title,
            Description = dto.Description,
            Link = dto.Link,
            PublishedAt = dto.PublishedAt,
            CreatedAt = DateTime.UtcNow
        });

        await _repository.BulkInsertIgnoreDuplicatesAsync(
            entities,
            cancellationToken);
    }

    private static NewsItemDto Map(NewsItem entity)
    {
        return new NewsItemDto
        {
            Id = entity.Id,
            RssChannelId = entity.RssChannelId,
            Title = entity.Title,
            Description = entity.Description,
            Link = entity.Link,
            PublishedAt = entity.PublishedAt,
            CreatedAt = entity.CreatedAt
        };
    }
}