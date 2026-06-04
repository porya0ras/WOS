namespace DesktopFramework.Components.Layout;

/// <summary>One entry in a <c>ContextMenu</c>. A separator has a null label.</summary>
public sealed record ContextMenuItem(string? Label, Action? Invoke = null, string? Icon = null)
{
    public bool IsSeparator => Label is null;
    public static ContextMenuItem Separator { get; } = new((string?)null);
}
