namespace DesktopFramework.Core.Windowing;

/// <summary>
/// Runtime state of one open window. Mutated only through <c>WindowManager</c>,
/// which is the single source of truth for windowing state.
/// </summary>
public sealed class WindowInstance
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string AppId { get; init; }

    /// <summary>Optional de-dup key for multi-instance apps (e.g. customer id).</summary>
    public string? InstanceKey { get; init; }

    public required Type ComponentType { get; init; }
    public IReadOnlyDictionary<string, object?> Parameters { get; init; } =
        new Dictionary<string, object?>();

    public string Title { get; set; } = "Window";
    public string? Icon { get; set; }

    public WindowState State { get; set; } = WindowState.Normal;
    public WindowPosition Position { get; set; }
    public WindowSize Size { get; set; }

    /// <summary>Bounds to restore to after un-maximizing. Captured on maximize.</summary>
    public WindowPosition? RestorePosition { get; set; }
    public WindowSize? RestoreSize { get; set; }

    public int ZIndex { get; set; }
    public bool IsActive { get; set; }

    public WindowOptions Options { get; init; } = WindowOptions.Default;

    public bool IsMinimized => State == WindowState.Minimized;
    public bool IsMaximized => State == WindowState.Maximized;
}
