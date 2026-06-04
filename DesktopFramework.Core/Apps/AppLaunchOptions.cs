using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Apps;

/// <summary>Per-launch overrides supplied when opening an app.</summary>
public sealed record AppLaunchOptions
{
    /// <summary>
    /// Parameters passed to the rendered component. Keys must match the target
    /// component's <c>[Parameter]</c> property names (see plan §10).
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Parameters { get; init; }

    public WindowOptions? WindowOverrides { get; init; }
    public string? TitleOverride { get; init; }

    /// <summary>Force a new window even for single-instance apps.</summary>
    public bool ForceNewInstance { get; init; }

    /// <summary>
    /// Optional key used to de-duplicate multi-instance windows (e.g. customer id).
    /// Opening the same key refocuses the existing window instead of duplicating.
    /// </summary>
    public string? InstanceKey { get; init; }
}
