using Home.Scout.Application;
using Home.Scout.Background.Rss;
using Home.Scout.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RssWorker>();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var host = builder.Build();
host.Run();
