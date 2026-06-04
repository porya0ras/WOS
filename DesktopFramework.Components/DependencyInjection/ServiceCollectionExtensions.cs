using DesktopFramework.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopFramework.Components.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the framework (Core services) for use by the desktop UI.
    /// The host must still register an <c>IDesktopPersistence</c> and an
    /// <c>IDesktopContentService</c> implementation (provided by the Web project).
    /// </summary>
    public static IServiceCollection AddDesktopFramework(this IServiceCollection services)
    {
        services.AddDesktopFrameworkCore();
        return services;
    }
}
