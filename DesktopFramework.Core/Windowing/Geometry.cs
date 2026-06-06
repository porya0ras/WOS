namespace DesktopFramework.Core.Windowing;

/// <summary>Window top-left position in CSS pixels relative to the desktop workspace.</summary>
public readonly record struct WindowPosition(double X, double Y);

/// <summary>Window size in CSS pixels.</summary>
public readonly record struct WindowSize(double Width, double Height);

/// <summary>A cell on the desktop icon grid (column, row).</summary>
public readonly record struct GridCell(int Col, int Row);

/// <summary>Commands a window can receive from chrome / context menus / taskbar.</summary>
public enum WindowCommand
{
    Focus,
    Minimize,
    Maximize,
    Restore,
    Close,
}
