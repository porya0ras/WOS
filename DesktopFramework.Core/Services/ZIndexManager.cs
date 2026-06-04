using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Single allocator for window z-index, so stacking order never relies on
/// ad-hoc values scattered through the UI (plan §21). Base value sits above
/// the desktop background but below the taskbar/menus, which use fixed bands.
/// </summary>
public sealed class ZIndexManager
{
    private const int Base = 100;
    private int _current = Base;

    /// <summary>Allocate the next top-most z-index and assign it to the window.</summary>
    public int BringToFront(WindowInstance window)
    {
        window.ZIndex = ++_current;
        return window.ZIndex;
    }

    /// <summary>Next z-index without binding it to a window.</summary>
    public int Next() => ++_current;
}
