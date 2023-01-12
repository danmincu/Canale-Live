using Canale_Live.Controllers;
using Canale_Live.Controllers.Getters;
using Microsoft.Extensions.Configuration.Json;

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("SETTINGS_CONFIG_PATH");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProxyGetter, ProxyGetter>();
builder.Services.AddSingleton<IRedirectCollection, Redirects>();


var fileName = !string.IsNullOrEmpty(configPath) ? Path.Combine(configPath, "appsettings.json") : "appsettings.json";
Console.WriteLine(fileName);
var tbr = builder.Configuration.Sources.OfType<JsonConfigurationSource>().Where(p => p.Path == "appsettings.json").FirstOrDefault();
if (tbr != null) builder.Configuration.Sources.Remove(tbr!);
builder.Configuration.AddJsonFile(fileName, optional: false, reloadOnChange: true);

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

//System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://localhost:5000") { UseShellExecute = true });

app.Run();

