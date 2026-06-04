using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Apps;

/// <summary>
/// A registered desktop app: maps a stable <see cref="Id"/> to the Blazor
/// component that renders inside its window, plus presentation and auth metadata.
/// </summary>
public sealed record AppDescriptor
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Icon { get; init; }

    /// <summary>The Blazor component rendered inside the window via DynamicComponent.</summary>
    public required Type ComponentType { get; init; }

    public DesktopAppCategory Category { get; init; } = DesktopAppCategory.Other;
    public bool AllowMultipleInstances { get; init; }

    public WindowOptions DefaultWindowOptions { get; init; } = WindowOptions.Default;

    public bool ShowOnDesktop { get; init; } = true;
    public bool ShowInStartMenu { get; init; } = true;
    public int Order { get; init; }

    // Auth-ready (enforced later by IPermissionService — see plan §18).
    public string[] RequiredRoles { get; init; } = [];
    public string[] RequiredClaims { get; init; } = [];
    public string[] RequiredPermissions { get; init; } = [];
}
