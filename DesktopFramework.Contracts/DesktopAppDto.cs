namespace DesktopFramework.Contracts;

/// <summary>
/// Metadata describing a launchable desktop app, as served by the backend.
/// Pure data — it carries no <see cref="System.Type"/>. The frontend maps
/// <see cref="Id"/> onto a registered component type (see AppRegistry).
/// </summary>
public sealed record DesktopAppDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Icon { get; init; }
    public string Category { get; init; } = "Other";
    public bool AllowMultipleInstances { get; init; }
    public int Order { get; init; }

    public string[] RequiredRoles { get; init; } = [];
    public string[] RequiredClaims { get; init; } = [];
    public string[] RequiredPermissions { get; init; } = [];
}
