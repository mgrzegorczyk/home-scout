using Home.Scout.Application.Channels.Dto;
using Home.Scout.Application.RssChannels;

namespace Home.Scout.Web.Tests.Fakes;

public class FakeChannelService : IChannelService
{
    private readonly List<RssChannelDto> _channels =
    [
        new()
        {
            Id = 1,
            Name = "Test Channel",
            Url = "https://example.com/feed.xml",
            Description = "A faked channel",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    ];

    public Task<IReadOnlyList<RssChannelDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RssChannelDto>>(_channels.ToList());

    public Task<RssChannelDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_channels.FirstOrDefault(c => c.Id == id));

    public Task<int> CreateAsync(CreateRssChannelDto dto, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task UpdateAsync(UpdateRssChannelDto dto, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
