var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    name: "Media3",
    pattern: "{controller=Media}/{a}/{b}/{c}",
    defaults: new { controller = "Media", action = "Index3"});

app.MapControllerRoute(
    name: "Media4",
    pattern: "{controller=Media}/{a}/{b}/{c}/{d}",
    defaults: new { controller = "Media", action = "Index4" });

app.MapControllerRoute(
    name: "Media9",
    pattern: "{controller=Media}/{a}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}",
    defaults: new { controller = "Media", action = "Index9" });

app.Run();
