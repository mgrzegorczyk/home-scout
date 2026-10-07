using Home.Scout.Application;
using Home.Scout.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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

public partial class Program;
