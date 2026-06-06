using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Client-side, observable notification store. Apps push notifications via
/// <see cref="Add(string, string, string)"/>; the notification panel renders the
/// list and dismisses items via <see cref="Dismiss"/>. Seeds once from the API
/// (<see cref="IDesktopContentService"/>) on first use. Scoped per circuit.
/// </summary>
public sealed class NotificationService
{
    private readonly IDesktopContentService _content;
    private readonly List<NotificationDto> _items = [];
    private bool _seeded;

    public NotificationService(IDesktopContentService content) => _content = content;

    /// <summary>Newest-first list of active notifications.</summary>
    public IReadOnlyList<NotificationDto> Notifications => _items;

    public int Count => _items.Count;

    /// <summary>Raised whenever the notification list changes (add/dismiss/clear).</summary>
    public event Action? Changed;

    /// <summary>Raised only when a new notification is pushed — used to show a toast.
    /// Seeding from the API does not raise this.</summary>
    public event Action<NotificationDto>? Added;

    /// <summary>Loads the backend's sample notifications once. Safe to call repeatedly.</summary>
    public async Task EnsureSeededAsync(CancellationToken ct = default)
    {
        if (_seeded) return;
        _seeded = true;

        // Failures are returned as data — on error we simply start empty.
        var result = await _content.GetNotificationsAsync(ct);
        if (!result.IsSuccess || result.Value is not { Count: > 0 } initial)
            return;

        // Keep newest-first ordering.
        foreach (var n in initial.OrderBy(n => n.Timestamp))
            _items.Insert(0, n);

        Changed?.Invoke();
    }

    public void Add(NotificationDto notification)
    {
        _items.Insert(0, notification);
        Added?.Invoke(notification);
        Changed?.Invoke();
    }

    public NotificationDto Add(string title, string message, string severity = "info")
    {
        var notification = new NotificationDto
        {
            Title = title,
            Message = message,
            Severity = severity,
            Timestamp = DateTimeOffset.Now,
        };
        Add(notification);
        return notification;
    }

    public void Dismiss(Guid id)
    {
        if (_items.RemoveAll(n => n.Id == id) > 0)
            Changed?.Invoke();
    }

    public void Clear()
    {
        if (_items.Count == 0) return;
        _items.Clear();
        Changed?.Invoke();
    }
}
