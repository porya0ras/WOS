using DesktopFramework.Components.Services;
using DesktopFramework.Core.DependencyInjection;
using DesktopFramework.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DesktopFramework.Components.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the full WOS web-desktop framework: the Core services plus the
    /// default browser-storage seams (localStorage / sessionStorage). After this the
    /// desktop runs out-of-the-box; register your own <c>IDesktopContentService</c> /
    /// <c>IAuthApiClient</c> (and optionally a role-based <c>IPermissionService</c>) to
    /// connect it to your backend. Call this in your Blazor app's service registration.
    /// </summary>
    public static IServiceCollection AddDesktopFramework(this IServiceCollection services)
    {
        services.AddDesktopFrameworkCore();

        // Default persistence (overridable).
        services.TryAddScoped<IDesktopPersistence, LocalStoragePersistence>();
        services.TryAddScoped<ISessionStore, SessionStoragePersistence>();

        return services;
    }
}
