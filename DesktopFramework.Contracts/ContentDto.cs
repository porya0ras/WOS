namespace DesktopFramework.Contracts;

/// <summary>A piece of desktop content (welcome text, about/system info, ...).</summary>
public sealed record ContentDto
{
    public required string Key { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public string? Html { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
