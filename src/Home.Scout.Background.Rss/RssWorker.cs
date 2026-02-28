using System.Collections.Concurrent;
using Home.Scout.Application.News;
using Home.Scout.Application.RssChannels;
using Home.Scout.Application.News.Dto;
using Home.Scout.Background.Rss.Services;

namespace Home.Scout.Background.Rss;

public sealed class RssWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RssWorker> _logger;

    public RssWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<RssWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessCycle(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RSS cycle failed");
            }
        }
    }

    private async Task ProcessCycle(CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();

        var rssService = scope.ServiceProvider.GetRequiredService<IRssFeedService>();
        var newsItemService = scope.ServiceProvider.GetRequiredService<INewsItemService>();
        var rssChannelService = scope.ServiceProvider.GetRequiredService<IChannelService>();
        var channels = await rssChannelService.GetAllAsync();

        var allItems = new ConcurrentBag<CreateNewsItemDto>();

        await Parallel.ForEachAsync(
            channels,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 6,
                CancellationToken = token
            },
            async (channel, ct) =>
            {
                var items = await rssService.FetchNewsAsync(channel);

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
            return;

        await newsItemService.BulkInsertIgnoreDuplicatesAsync(allItems);
    }
}