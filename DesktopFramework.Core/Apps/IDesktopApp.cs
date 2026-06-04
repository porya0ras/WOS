using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Apps;

/// <summary>
/// Optional interface a windowed component may implement to react to its
/// host window's commands/lifecycle. Plain pages need not implement it.
/// </summary>
public interface IDesktopApp
{
    /// <summary>The window hosting this component. Set by the framework on render.</summary>
    WindowInstance? Window { get; set; }

    /// <summary>Called when a window command targets this app (close, maximize, ...).</summary>
    void OnWindowCommand(WindowCommand command) { }
}
