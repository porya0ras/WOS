using DesktopFramework.Components.DependencyInjection;
using DesktopFramework.Core;
using DesktopFramework.Core.Services;
using DesktopFramework.Web.Components;
using DesktopFramework.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Framework: windowing, registry, theme, notifications, auth orchestration, and the
// default browser-storage seams (localStorage / sessionStorage).
builder.Services.AddDesktopFramework();

// Login flow config (two-step by default; set EnableContextStep = false for single-step).
builder.Services.AddSingleton(new LoginFlowOptions
{
    EnableContextStep = true,
    ShowCompanySelector = true,
    ShowRoleSelector = true,
});

// Enable role-based access (overrides the framework's default allow-all).
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Typed API client, resolved against the Aspire-discovered "api" service.
builder.Services.AddHttpClient<DesktopApiClient>(client =>
    client.BaseAddress = new Uri("https+http://api"));
builder.Services.AddScoped<IDesktopContentService>(sp => sp.GetRequiredService<DesktopApiClient>());

// Diagnostics client (used by the API Tester sample app).
builder.Services.AddHttpClient<DiagnosticsApiClient>(client =>
    client.BaseAddress = new Uri("https+http://api"));
builder.Services.AddScoped<IDiagnosticsService>(sp => sp.GetRequiredService<DiagnosticsApiClient>());

// Auth client.
builder.Services.AddHttpClient<AuthApiClient>(client =>
    client.BaseAddress = new Uri("https+http://api"));
builder.Services.AddScoped<IAuthApiClient>(sp => sp.GetRequiredService<AuthApiClient>());

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
