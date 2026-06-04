namespace DesktopFramework.Contracts;

/// <summary>A single desktop notification.</summary>
public sealed record NotificationDto
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; init; }
    public required string Message { get; init; }

    /// <summary>One of: info | success | warning | error.</summary>
    public string Severity { get; init; } = "info";

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public bool IsRead { get; init; }
}
