using Home.Scout.Application.Rss;

namespace Home.Scout.Web.Tests.Fakes;

public class FakeRssSyncService : IRssSyncService
{
    public Task<int> SyncChannelAsync(int channelId, CancellationToken cancellationToken = default)
        => Task.FromResult(0);

    public Task<int> SyncAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(0);
}
