using DesktopFramework.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DesktopFramework.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the framework's core services. All are Scoped so each Blazor
    /// circuit gets its own desktop. Sensible defaults are registered with TryAdd so
    /// the desktop runs out-of-the-box; override any of them by registering your own:
    /// <list type="bullet">
    /// <item><see cref="IPermissionService"/> → allow-all (register PermissionService for role gating)</item>
    /// <item><see cref="IDesktopContentService"/> → empty/no-op (register an API-backed client)</item>
    /// <item><see cref="IAuthApiClient"/> → offline/no-op (register an API-backed client)</item>
    /// <item><see cref="LoginFlowOptions"/> → two-step defaults</item>
    /// </list>
    /// The browser-storage seams (<see cref="IDesktopPersistence"/>, <see cref="ISessionStore"/>)
    /// are provided by the Components package via <c>AddDesktopFramework()</c>.
    /// </summary>
    public static IServiceCollection AddDesktopFrameworkCore(this IServiceCollection services)
    {
        // Overridable defaults.
        services.TryAddScoped<IPermissionService, AllowAllPermissionService>();
        services.TryAddScoped<IDesktopContentService, EmptyContentService>();
        services.TryAddScoped<IAuthApiClient, OfflineAuthApiClient>();
        services.TryAddSingleton<LoginFlowOptions>();

        // Framework services.
        services.AddScoped<AppRegistry>();
        services.AddScoped<ZIndexManager>();
        services.AddScoped<WindowManager>();
        services.AddScoped<DesktopStateService>();
        services.AddScoped<BusyService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<ThemeService>();
        services.AddScoped<WindowPersistenceService>();
        services.TryAddScoped<AuthService>();

        return services;
    }
}
