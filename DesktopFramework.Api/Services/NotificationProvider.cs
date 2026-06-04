using DesktopFramework.Contracts;

namespace DesktopFramework.Api.Services;

/// <summary>In-memory source of sample notifications. Singleton for v1.</summary>
public sealed class NotificationProvider
{
    private readonly IReadOnlyList<NotificationDto> _notifications =
    [
        new() { Title = "Welcome",        Message = "Your web desktop is ready.",            Severity = "success", Timestamp = DateTimeOffset.UtcNow.AddMinutes(-1) },
        new() { Title = "Update",         Message = "WOS framework v1 is running.",          Severity = "info",    Timestamp = DateTimeOffset.UtcNow.AddMinutes(-10) },
        new() { Title = "Tip",            Message = "Drag a window's title bar to move it.", Severity = "info",    Timestamp = DateTimeOffset.UtcNow.AddMinutes(-30) },
    ];

    public IReadOnlyList<NotificationDto> GetNotifications() => _notifications;
}
