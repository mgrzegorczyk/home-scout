using Home.Scout.Application.News;
using Home.Scout.Application.RssChannels;
using Home.Scout.Infrastructure.News;
using Home.Scout.Infrastructure.RssChannels;
using Microsoft.Extensions.DependencyInjection;

namespace Home.Scout.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IRssChannelRepository, RssChannelRepository>();
        services.AddScoped<INewsItemRepository, NewsItemRepository>();

        return services;
    }
}