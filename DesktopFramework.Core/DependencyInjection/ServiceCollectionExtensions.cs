using DesktopFramework.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DesktopFramework.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the framework's core services. All are Scoped so each Blazor
    /// circuit gets its own desktop (plan §21). The Web project supplies the
    /// <see cref="IDesktopPersistence"/> and <see cref="IDesktopContentService"/>
    /// implementations; a default permission service is registered if none exists.
    /// </summary>
    public static IServiceCollection AddDesktopFrameworkCore(this IServiceCollection services)
    {
        services.TryAddScoped<IPermissionService, AllowAllPermissionService>();

        services.AddScoped<AppRegistry>();
        services.AddScoped<ZIndexManager>();
        services.AddScoped<WindowManager>();
        services.AddScoped<DesktopStateService>();
        services.AddScoped<BusyService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<ThemeService>();
        services.AddScoped<WindowPersistenceService>();

        return services;
    }
}
