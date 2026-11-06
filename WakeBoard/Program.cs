using WakeBoard.Models;
using WakeBoard.Services;

// Create web application builder.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.Configure<MachineOptions>(builder.Configuration.GetSection("WakeBoard"));
builder.Services.AddSingleton<MachineStatusService>();
builder.Services.AddSingleton<WakeOnLanService>();

// Create the web application and configure.
WebApplication app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseRouting();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();