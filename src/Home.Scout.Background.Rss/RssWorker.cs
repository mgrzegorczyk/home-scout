using Home.Scout.Application.Rss;

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

        var rssSyncService = scope.ServiceProvider.GetRequiredService<IRssSyncService>();

        await rssSyncService.SyncAllAsync(token);
    }
}
