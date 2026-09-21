using System.Collections.Concurrent;
using Home.Scout.Application.Channels.Dto;
using Home.Scout.Application.News;
using Home.Scout.Application.News.Dto;
using Home.Scout.Application.RssChannels;

namespace Home.Scout.Application.Rss;

public interface IRssSyncService
{
    Task<int> SyncChannelAsync(int channelId, CancellationToken cancellationToken = default);
    Task<int> SyncAllAsync(CancellationToken cancellationToken = default);
}

public class RssSyncService : IRssSyncService
{
    private const int MaxDegreeOfParallelism = 6;

    private readonly IRssFeedService _rssFeedService;
    private readonly IChannelService _channelService;
    private readonly INewsItemService _newsItemService;

    public RssSyncService(
        IRssFeedService rssFeedService,
        IChannelService channelService,
        INewsItemService newsItemService)
    {
        _rssFeedService = rssFeedService;
        _channelService = channelService;
        _newsItemService = newsItemService;
    }

    public async Task<int> SyncChannelAsync(int channelId, CancellationToken cancellationToken = default)
    {
        var channel = await _channelService.GetByIdAsync(channelId, cancellationToken)
            ?? throw new KeyNotFoundException($"Channel with Id {channelId} not found.");

        return await SyncChannelsAsync([channel], cancellationToken);
    }

    public async Task<int> SyncAllAsync(CancellationToken cancellationToken = default)
    {
        var channels = await _channelService.GetAllAsync(cancellationToken);

        return await SyncChannelsAsync(channels, cancellationToken);
    }

    private async Task<int> SyncChannelsAsync(
        IReadOnlyList<RssChannelDto> channels,
        CancellationToken cancellationToken)
    {
        var allItems = new ConcurrentBag<CreateNewsItemDto>();

        await Parallel.ForEachAsync(
            channels,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                CancellationToken = cancellationToken
            },
            async (channel, ct) =>
            {
                var items = await _rssFeedService.FetchNewsAsync(channel);

                foreach (var item in items)
                {
                    allItems.Add(new CreateNewsItemDto
                    {
                        RssChannelId = item.RssChannelId,
                        Title = item.Title,
                        Description = item.Description,
                        Link = item.Link,
                        PublishedAt = item.PublishedAt,
                    });
                }
            });

        if (allItems.IsEmpty)
            return 0;

        await _newsItemService.BulkInsertIgnoreDuplicatesAsync(allItems, cancellationToken);

        return allItems.Count;
    }
}
