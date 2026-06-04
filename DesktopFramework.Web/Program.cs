using DesktopFramework.Components.DependencyInjection;
using DesktopFramework.Core.Services;
using DesktopFramework.Web.Components;
using DesktopFramework.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Framework core services (windowing, registry, theme, persistence orchestration).
builder.Services.AddDesktopFramework();

// Host-supplied implementations of the framework's seams.
builder.Services.AddScoped<IDesktopPersistence, LocalStoragePersistence>();

// Typed API client, resolved against the Aspire-discovered "api" service.
builder.Services.AddHttpClient<DesktopApiClient>(client =>
    client.BaseAddress = new Uri("https+http://api"));
builder.Services.AddScoped<IDesktopContentService>(sp => sp.GetRequiredService<DesktopApiClient>());

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
