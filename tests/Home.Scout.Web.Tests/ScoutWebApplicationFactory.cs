using Home.Scout.Application.News;
using Home.Scout.Application.Rss;
using Home.Scout.Application.RssChannels;
using Home.Scout.Web.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Home.Scout.Web.Tests;

/// <summary>
/// Hosts the web app in memory with Application services replaced by fakes, so no Postgres is needed.
/// </summary>
public class ScoutWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeChannelService Channels { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IChannelService>();
            services.RemoveAll<IRssSyncService>();
            services.RemoveAll<INewsItemService>();

            services.AddSingleton<IChannelService>(Channels);
            services.AddSingleton<IRssSyncService, FakeRssSyncService>();
            services.AddSingleton<INewsItemService, FakeNewsItemService>();
        });
    }

    public HttpClient CreateNonRedirectingClient()
        => CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
}
