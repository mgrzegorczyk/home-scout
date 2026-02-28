using Home.Scout.Application.Channels.Dto;
using Home.Scout.Domain.Models;

namespace Home.Scout.Application.RssChannels;

public interface IChannelService
{
    Task<IReadOnlyList<RssChannelDto>> GetAllAsync();
    Task<RssChannelDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateRssChannelDto dto);
    Task UpdateAsync(UpdateRssChannelDto dto);
    Task DeleteAsync(int id);
}

public class ChannelService : IChannelService
{
    private readonly IRssChannelRepository _repository;

    public ChannelService(IRssChannelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RssChannelDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();

        return entities
            .Select(MapToDto)
            .ToList();
    }

    public async Task<RssChannelDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);

        return entity is null
            ? null
            : MapToDto(entity);
    }

    public async Task<int> CreateAsync(CreateRssChannelDto dto)
    {
        ValidateCreate(dto);

        var entity = new RssChannel
        {
            Name = dto.Name.Trim(),
            Url = dto.Url.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        return entity.Id;
    }

    public async Task UpdateAsync(UpdateRssChannelDto dto)
    {
        ValidateUpdate(dto);

        var entity = await _repository.GetByIdAsync(dto.Id)
                     ?? throw new KeyNotFoundException($"Channel with Id {dto.Id} not found.");

        entity.Name = dto.Name.Trim();
        entity.Url = dto.Url.Trim();
        entity.Description = dto.Description?.Trim() ?? string.Empty;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
                     ?? throw new KeyNotFoundException($"Channel with Id {id} not found.");

        await _repository.DeleteAsync(entity);
    }

    private static RssChannelDto MapToDto(RssChannel entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Url = entity.Url,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

    private static void ValidateCreate(CreateRssChannelDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Channel name is required.");

        if (string.IsNullOrWhiteSpace(dto.Url))
            throw new ArgumentException("Channel URL is required.");

        if (!Uri.TryCreate(dto.Url, UriKind.Absolute, out _))
            throw new ArgumentException("Channel URL must be a valid absolute URI.");
    }

    private static void ValidateUpdate(UpdateRssChannelDto dto)
    {
        if (dto.Id <= 0)
            throw new ArgumentException("Invalid channel Id.");

        ValidateCreate(new CreateRssChannelDto
        {
            Name = dto.Name,
            Url = dto.Url,
            Description = dto.Description
        });
    }
}