using Home.Scout.Application;
using Home.Scout.Background.Rss;
using Home.Scout.Background.Rss.Services;
using Home.Scout.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RssWorker>();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddHttpClient<IRssFeedService, RssFeedService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

var host = builder.Build();
host.Run();