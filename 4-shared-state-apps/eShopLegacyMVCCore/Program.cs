
using eShopLegacy.Models;
using eShopLegacyMVCCore.Models;
using eShopLegacyMVCCore.Models.Infrastructure;
using eShopLegacyMVCCore.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SystemWebAdapters.Authentication;
using System.Data.Entity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSystemWebAdapters()
    .AddJsonSessionSerializer(options =>
    {
        // Serialization/deserialization requires each session key to be registered to a type
        options.RegisterKey<int>("test-value");
        options.RegisterKey<string>("MachineName");
        options.RegisterKey<DateTime>("SessionStartTime");
        options.RegisterKey<SessionDemoModel>("DemoItem");
    })
    .AddRemoteAppClient(options =>
    {
        // Provide the URL for the remote app that has enabled session querying
        options.RemoteAppUrl = new(builder.Configuration["ReverseProxy:Clusters:fallbackCluster:Destinations:fallbackApp:Address"]);
        //options.RemoteAppUrl = new("https://localhost:44319");
        // Provide a strong API key that will be used to authenticate the request on the remote app for querying the session
        options.ApiKey = builder.Configuration["RemoteAppApiKey"];
    })
    // Setup authentication handler for shared login state
    .AddAuthenticationClient(true)
    .AddSessionClient();

// Shared cookie authentication
var keyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DPKeys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
    .SetApplicationName("eShop");
builder.Services.AddAuthentication(RemoteAppAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.Name = ".AspNet.ApplicationCookie";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Path = "/";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    });

// TODO: Do not hardcode this
var mockData = bool.Parse("false");
if (mockData)
{
    builder.Services.AddScoped<ICatalogService, CatalogServiceMock>();
}
else
{
    builder.Services.AddScoped<ICatalogService, CatalogService>();
}
builder.Services.AddScoped<CatalogDBContext>();
builder.Services.AddScoped<CatalogDBInitializer>();
builder.Services.AddSingleton<CatalogItemHiLoGenerator>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
// TODO: Change logging library
builder.Logging.AddLog4Net("log4Net.xml");

var app = builder.Build();
if (!mockData)
{
    using var scope = app.Services.CreateScope();
    Database.SetInitializer(scope.ServiceProvider.GetRequiredService<CatalogDBInitializer>());
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSystemWebAdapters();

app.MapControllerRoute("Default", "{controller=Catalog}/{action=Index}/{id?}")
    // Enable session support for all routes
    .RequireSystemWebAdapterSession();

app.MapReverseProxy();

app.Run();
