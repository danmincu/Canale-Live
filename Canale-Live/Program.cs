using Canale_Live.Controllers;
using Canale_Live.Controllers.Getters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProxyGetter, ProxyGetter>();
builder.Services.AddSingleton<IRedirectCollection, Redirects>();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

System.Net.ServicePointManager.DefaultConnectionLimit = 30;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Media4",
    pattern: "{controller=Media}/{a}/{b}/{c}/{d?}",
    defaults: new { controller = "Media", action = "Index4" });

app.MapControllerRoute(
    name: "Media9",
    pattern: "{controller=Media}/{a}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}",
    defaults: new { controller = "Media", action = "Index9" });

app.Run();

