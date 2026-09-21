using Home.Scout.Application.News;
using Home.Scout.Application.Rss;
using Home.Scout.Application.RssChannels;
using Microsoft.Extensions.DependencyInjection;

namespace Home.Scout.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IChannelService, ChannelService>();
        services.AddScoped<INewsItemService, NewsItemService>();
        services.AddScoped<IRssSyncService, RssSyncService>();

        services.AddHttpClient<IRssFeedService, RssFeedService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}