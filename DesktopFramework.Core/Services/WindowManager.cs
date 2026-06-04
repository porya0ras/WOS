using DesktopFramework.Core.Apps;
using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Central windowing API and single source of truth for open windows. The UI
/// mutates windowing state only through this service; it raises <see cref="Changed"/>
/// so the shell re-renders. Scoped (one desktop per circuit).
/// </summary>
public sealed class WindowManager
{
    private readonly AppRegistry _registry;
    private readonly ZIndexManager _zIndex;
    private readonly IPermissionService _permissions;
    private readonly List<WindowInstance> _windows = [];

    private const double CascadeStep = 28;
    private double _cascadeOffset;

    public WindowManager(AppRegistry registry, ZIndexManager zIndex, IPermissionService permissions)
    {
        _registry = registry;
        _zIndex = zIndex;
        _permissions = permissions;
    }

    public IReadOnlyList<WindowInstance> Windows => _windows;
    public Guid? ActiveWindowId { get; private set; }

    /// <summary>Raised after any windowing state change.</summary>
    public event Action? Changed;

    public WindowInstance? OpenApp(string appId, AppLaunchOptions? options = null)
    {
        var descriptor = _registry.Get(appId);
        if (descriptor is null || !_permissions.CanLaunch(descriptor))
            return null;

        // Refocus an existing window when single-instance, or when an instance
        // key matches (multi-instance de-dup) — unless a new instance is forced.
        if (!options?.ForceNewInstance ?? true)
        {
            var existing = FindReusable(descriptor, options);
            if (existing is not null)
            {
                Focus(existing.Id);
                return existing;
            }
        }

        var opts = options?.WindowOverrides ?? descriptor.DefaultWindowOptions;

        var window = new WindowInstance
        {
            AppId = descriptor.Id,
            InstanceKey = options?.InstanceKey,
            ComponentType = descriptor.ComponentType,
            Parameters = options?.Parameters ?? new Dictionary<string, object?>(),
            Title = options?.TitleOverride ?? descriptor.Title,
            Icon = descriptor.Icon,
            Options = opts,
            Size = opts.InitialSize,
            Position = opts.InitialPosition ?? NextCascadePosition(),
            State = opts.StartMaximized ? WindowState.Maximized : WindowState.Normal,
        };

        _windows.Add(window);
        FocusInternal(window);
        Changed?.Invoke();
        return window;
    }

    public void Close(Guid id)
    {
        var window = Find(id);
        if (window is null) return;

        _windows.Remove(window);

        if (ActiveWindowId == id)
        {
            var top = _windows.Where(w => !w.IsMinimized).OrderByDescending(w => w.ZIndex).FirstOrDefault();
            if (top is not null) FocusInternal(top);
            else ActiveWindowId = null;
        }

        Changed?.Invoke();
    }

    public void Focus(Guid id)
    {
        var window = Find(id);
        if (window is null) return;

        // Focusing a minimized window restores it.
        if (window.IsMinimized) window.State = WindowState.Normal;

        FocusInternal(window);
        Changed?.Invoke();
    }

    public void Minimize(Guid id)
    {
        var window = Find(id);
        if (window is null || !window.Options.Minimizable) return;

        window.State = WindowState.Minimized;
        window.IsActive = false;

        if (ActiveWindowId == id)
        {
            var top = _windows.Where(w => !w.IsMinimized).OrderByDescending(w => w.ZIndex).FirstOrDefault();
            ActiveWindowId = top?.Id;
            if (top is not null) top.IsActive = true;
        }

        Changed?.Invoke();
    }

    public void Maximize(Guid id)
    {
        var window = Find(id);
        if (window is null || !window.Options.Maximizable) return;

        if (window.State == WindowState.Normal)
        {
            window.RestorePosition = window.Position;
            window.RestoreSize = window.Size;
        }
        window.State = WindowState.Maximized;
        FocusInternal(window);
        Changed?.Invoke();
    }

    public void Restore(Guid id)
    {
        var window = Find(id);
        if (window is null) return;

        if (window.State == WindowState.Maximized)
        {
            if (window.RestorePosition is { } p) window.Position = p;
            if (window.RestoreSize is { } s) window.Size = s;
        }
        window.State = WindowState.Normal;
        FocusInternal(window);
        Changed?.Invoke();
    }

    public void ToggleMaximize(Guid id)
    {
        var window = Find(id);
        if (window is null) return;
        if (window.IsMaximized) Restore(id);
        else Maximize(id);
    }

    public void Move(Guid id, WindowPosition position)
    {
        var window = Find(id);
        if (window is null || !window.Options.Movable) return;

        window.Position = position;
        Changed?.Invoke();
    }

    public void Resize(Guid id, WindowSize size)
    {
        var window = Find(id);
        if (window is null || !window.Options.Resizable) return;

        window.Size = Clamp(size, window.Options);
        Changed?.Invoke();
    }

    public void Resize(Guid id, WindowPosition position, WindowSize size)
    {
        var window = Find(id);
        if (window is null || !window.Options.Resizable) return;

        window.Position = position;
        window.Size = Clamp(size, window.Options);
        Changed?.Invoke();
    }

    private WindowInstance? Find(Guid id) => _windows.FirstOrDefault(w => w.Id == id);

    private WindowInstance? FindReusable(AppDescriptor descriptor, AppLaunchOptions? options)
    {
        if (!descriptor.AllowMultipleInstances)
            return _windows.FirstOrDefault(w => w.AppId == descriptor.Id);

        return options?.InstanceKey is { } key
            ? _windows.FirstOrDefault(w => w.AppId == descriptor.Id && w.InstanceKey == key)
            : null;
    }

    private void FocusInternal(WindowInstance window)
    {
        _zIndex.BringToFront(window);
        foreach (var w in _windows) w.IsActive = false;
        window.IsActive = true;
        ActiveWindowId = window.Id;
    }

    private WindowPosition NextCascadePosition()
    {
        _cascadeOffset = (_cascadeOffset + CascadeStep) % (CascadeStep * 8);
        return new WindowPosition(40 + _cascadeOffset, 40 + _cascadeOffset);
    }

    private static WindowSize Clamp(WindowSize size, WindowOptions options)
    {
        var w = Math.Max(size.Width, options.MinSize.Width);
        var h = Math.Max(size.Height, options.MinSize.Height);
        if (options.MaxSize is { } max)
        {
            w = Math.Min(w, max.Width);
            h = Math.Min(h, max.Height);
        }
        return new WindowSize(w, h);
    }
}
