namespace DesktopFramework.Core.Services;

/// <summary>
/// Observable shell-level UI state (start menu / notification panel visibility,
/// active context menu). Windowing lives in <see cref="WindowManager"/>; this
/// holds the rest of the desktop chrome state. Scoped per circuit.
/// </summary>
public sealed class DesktopStateService
{
    public bool IsStartMenuOpen { get; private set; }
    public bool IsNotificationPanelOpen { get; private set; }

    public event Action? Changed;

    public void ToggleStartMenu()
    {
        IsStartMenuOpen = !IsStartMenuOpen;
        if (IsStartMenuOpen) IsNotificationPanelOpen = false;
        Changed?.Invoke();
    }

    public void ToggleNotificationPanel()
    {
        IsNotificationPanelOpen = !IsNotificationPanelOpen;
        if (IsNotificationPanelOpen) IsStartMenuOpen = false;
        Changed?.Invoke();
    }

    /// <summary>Close all transient popups (start menu, panels). Called on desktop click.</summary>
    public void CloseAllMenus()
    {
        if (!IsStartMenuOpen && !IsNotificationPanelOpen) return;
        IsStartMenuOpen = false;
        IsNotificationPanelOpen = false;
        Changed?.Invoke();
    }
}
