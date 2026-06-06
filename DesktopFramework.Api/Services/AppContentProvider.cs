using DesktopFramework.Contracts;

namespace DesktopFramework.Api.Services;

/// <summary>In-memory source of app catalog + content. Singleton for v1.</summary>
public sealed class AppContentProvider
{
    private readonly IReadOnlyList<DesktopAppDto> _apps =
    [
        new() { Id = "welcome",   Title = "Welcome",   Icon = "fa-solid fa-hands-clapping", Category = "System",       Order = 0 },
        new() { Id = "about",     Title = "About",     Icon = "fa-solid fa-circle-info",    Category = "System",       Order = 1 },
        new() { Id = "settings",  Title = "Settings",  Icon = "fa-solid fa-gear",           Category = "System",       Order = 2 },
        new() { Id = "customers", Title = "Customers", Icon = "fa-solid fa-user",           Category = "Productivity", Order = 3, AllowMultipleInstances = true },
        new() { Id = "notes",     Title = "Notes",     Icon = "fa-solid fa-note-sticky",    Category = "Productivity", Order = 4, AllowMultipleInstances = true },
    ];

    private readonly Dictionary<string, ContentDto> _content = new(StringComparer.OrdinalIgnoreCase)
    {
        ["welcome"] = new ContentDto
        {
            Key = "welcome",
            Title = "Welcome to WOS",
            Body = "A Blazor-based web desktop. Open apps from the desktop icons or the Start menu — each opens in a movable, resizable window.",
            Html = "<p>Welcome to <strong>WOS</strong>, a web desktop built with Blazor &amp; .NET Aspire.</p><p>Double-click an icon or use the Start menu to launch an app.</p>",
            UpdatedAt = DateTimeOffset.UtcNow,
        },
        ["about"] = new ContentDto
        {
            Key = "about",
            Title = "About WOS",
            Body = "WOS — Web Operating Shell. Built on .NET 10, Blazor (Interactive Server) and .NET Aspire. Content served by DesktopFramework.Api.",
            Html = "<h3>WOS</h3><ul><li>Runtime: .NET 10</li><li>UI: Blazor Interactive Server</li><li>Orchestration: .NET Aspire</li></ul>",
            UpdatedAt = DateTimeOffset.UtcNow,
        },
    };

    public IReadOnlyList<DesktopAppDto> GetApps() => _apps;

    public ContentDto? GetContent(string key) =>
        _content.TryGetValue(key, out var c) ? c : null;

    /// <summary>Freshly generated "report" — served behind a deliberate delay (slow endpoint).</summary>
    public ContentDto GetReport() => new()
    {
        Key = "report",
        Title = "Sales Report",
        Body = $"Loaded {Random.Shared.Next(120, 980)} records from the server at {DateTimeOffset.Now:HH:mm:ss}.",
        UpdatedAt = DateTimeOffset.Now,
    };
}
