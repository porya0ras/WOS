using System.Collections.Concurrent;
using DesktopFramework.Core.Apps;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Holds the catalog of registered apps and answers filtered queries used by the
/// desktop, start menu and window manager. Registered at startup.
/// </summary>
public sealed class AppRegistry
{
    private readonly ConcurrentDictionary<string, AppDescriptor> _apps = new(StringComparer.OrdinalIgnoreCase);
    private readonly IPermissionService _permissions;

    public AppRegistry(IPermissionService permissions) => _permissions = permissions;

    public event Action? Changed;

    public void Register(AppDescriptor descriptor)
    {
        _apps[descriptor.Id] = descriptor;
        Changed?.Invoke();
    }

    public void RegisterAll(IEnumerable<AppDescriptor> descriptors)
    {
        foreach (var d in descriptors)
            _apps[d.Id] = d;
        Changed?.Invoke();
    }

    public AppDescriptor? Get(string id) =>
        _apps.TryGetValue(id, out var d) ? d : null;

    public IReadOnlyList<AppDescriptor> All =>
        _apps.Values.OrderBy(a => a.Order).ThenBy(a => a.Title).ToList();

    /// <summary>Apps the current user is allowed to see.</summary>
    public IEnumerable<AppDescriptor> Visible =>
        All.Where(_permissions.CanLaunch);

    public IEnumerable<AppDescriptor> DesktopApps =>
        Visible.Where(a => a.ShowOnDesktop);

    public IEnumerable<AppDescriptor> StartMenuApps =>
        Visible.Where(a => a.ShowInStartMenu);
}
