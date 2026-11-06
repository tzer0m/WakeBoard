using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using WakeBoard.Models;
using WakeBoard.Services;

// Create web application builder.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.Configure<MachineOptions>(builder.Configuration.GetSection("WakeBoard"));
builder.Services.AddSingleton<MachineStatusService>();
builder.Services.AddSingleton<WakeOnLanService>();
AuthenticationBuilder authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
});
authentication.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});
authentication.AddOpenIdConnect(options =>
{
    options.Authority = builder.Configuration["Oidc:Authority"];
    options.ClientId = builder.Configuration["Oidc:ClientId"];
    options.ClientSecret = builder.Configuration["Oidc:ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
});
builder.Services.AddAuthorization();

// Create the web application and configure.
WebApplication app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapHealthChecks("/health");
app.Run();