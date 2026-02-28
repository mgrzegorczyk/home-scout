using Home.Scout.Application.News.Dto;
using Home.Scout.Domain.Models;

namespace Home.Scout.Application.News;

public interface INewsItemService
{
    Task<IReadOnlyList<NewsItemDto>> GetAllAsync();
    Task<IReadOnlyList<NewsItemDto>> GetByChannelIdAsync(int channelId);
    Task<NewsItemDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateNewsItemDto dto);
    Task<bool> UpdateAsync(int id, UpdateNewsItemDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByChannelIdAsync(int channelId);

    Task BulkInsertIgnoreDuplicatesAsync(IEnumerable<CreateNewsItemDto> dtos);
}

public class NewsItemService : INewsItemService
{
    private readonly INewsItemRepository _repository;

    public NewsItemService(INewsItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<NewsItemDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<NewsItemDto>> GetByChannelIdAsync(int channelId)
    {
        var items = await _repository.GetByChannelIdAsync(channelId);
        return items.Select(Map).ToList();
    }

    public async Task<NewsItemDto?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item == null ? null : Map(item);
    }

    public async Task<int> CreateAsync(CreateNewsItemDto dto)
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

        return await _repository.AddAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateNewsItemDto dto)
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

        return await _repository.UpdateAsync(entity);
    }

    public Task<bool> DeleteAsync(int id)
        => _repository.DeleteAsync(id);

    public Task<bool> DeleteByChannelIdAsync(int channelId)
        => _repository.DeleteByChannelIdAsync(channelId);

    public async Task BulkInsertIgnoreDuplicatesAsync(IEnumerable<CreateNewsItemDto> dtos)
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

        await _repository.BulkInsertIgnoreDuplicatesAsync(entities);
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