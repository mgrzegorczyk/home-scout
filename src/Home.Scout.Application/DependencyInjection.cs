using Home.Scout.Application.News;
using Home.Scout.Application.RssChannels;
using Microsoft.Extensions.DependencyInjection;

namespace Home.Scout.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IChannelService, ChannelService>();
        services.AddScoped<INewsItemService, NewsItemService>();

        return services;
    }
}