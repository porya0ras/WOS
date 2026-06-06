using System.Net.Http.Json;
using DesktopFramework.Contracts;
using DesktopFramework.Core.Services;

namespace DesktopFramework.Web.Services;

/// <summary>
/// Typed client for DesktopFramework.Api. The HttpClient's BaseAddress is the
/// Aspire logical name ("https+http://api"); service discovery resolves it.
/// </summary>
public sealed class DesktopApiClient : IDesktopContentService
{
    private readonly HttpClient _http;

    public DesktopApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<DesktopAppDto>> GetAppsAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<DesktopAppDto>>("/api/apps", ct) ?? [];

    public Task<ContentDto?> GetWelcomeAsync(CancellationToken ct = default) =>
        _http.GetFromJsonAsync<ContentDto>("/api/content/welcome", ct);

    public Task<ContentDto?> GetAboutAsync(CancellationToken ct = default) =>
        _http.GetFromJsonAsync<ContentDto>("/api/content/about", ct);

    public Task<ContentDto?> GetReportAsync(CancellationToken ct = default) =>
        _http.GetFromJsonAsync<ContentDto>("/api/content/report", ct);

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<NotificationDto>>("/api/notifications", ct) ?? [];
}
