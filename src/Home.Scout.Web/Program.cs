using Home.Scout.Web.Data;
using Home.Scout.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register Data Access Services
builder.Services.AddScoped<IRssChannelRepository, RssChannelRepository>();
builder.Services.AddScoped<INewsItemRepository, NewsItemRepository>();

// Register Business Logic Services
builder.Services.AddHttpClient<IRssFeedService, RssFeedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/News");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();
