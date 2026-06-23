using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VolcanREG;
using VolcanREG.Models;
using VolcanREG.Security;
using VolcanREG.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var firebaseOptions = new FirebaseOptions
{
    ApiKey = builder.Configuration["Firebase:ApiKey"] ?? string.Empty,
    AuthDomain = builder.Configuration["Firebase:AuthDomain"] ?? string.Empty,
    ProjectId = builder.Configuration["Firebase:ProjectId"] ?? string.Empty,
    StorageBucket = builder.Configuration["Firebase:StorageBucket"] ?? string.Empty,
    MessagingSenderId = builder.Configuration["Firebase:MessagingSenderId"] ?? string.Empty,
    AppId = builder.Configuration["Firebase:AppId"] ?? string.Empty
};

builder.Services.AddSingleton(firebaseOptions);
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IndexedDbService>();
builder.Services.AddScoped<ConnectivityService>();
builder.Services.AddScoped<GeolocationService>();
builder.Services.AddScoped<SyncService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<ChartService>();
builder.Services.AddScoped<RoleGuard>();

await builder.Build().RunAsync();
