namespace DesktopFramework.Core.Windowing;

/// <summary>Creation-time configuration for a window.</summary>
public sealed record WindowOptions
{
    public WindowSize InitialSize { get; init; } = new(640, 460);

    /// <summary>Null = let the WindowManager cascade/center the window.</summary>
    public WindowPosition? InitialPosition { get; init; }

    public WindowSize MinSize { get; init; } = new(240, 160);
    public WindowSize? MaxSize { get; init; }

    public bool Resizable { get; init; } = true;
    public bool Movable { get; init; } = true;
    public bool ShowInTaskbar { get; init; } = true;
    public bool Maximizable { get; init; } = true;
    public bool Minimizable { get; init; } = true;
    public bool Closable { get; init; } = true;
    public bool StartMaximized { get; init; }

    public static WindowOptions Default { get; } = new();
}
