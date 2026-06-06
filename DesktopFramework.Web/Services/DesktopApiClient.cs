using System.Net.Http.Json;
using DesktopFramework.Contracts;
using DesktopFramework.Core;
using DesktopFramework.Core.Services;

namespace DesktopFramework.Web.Services;

/// <summary>
/// Typed client for DesktopFramework.Api. The HttpClient's BaseAddress is the
/// Aspire logical name ("https+http://api"); service discovery resolves it.
/// Calls are wrapped by <see cref="ApiCall"/> so failures come back as
/// <see cref="ApiResult{T}"/> — never an exception.
/// </summary>
public sealed class DesktopApiClient : IDesktopContentService
{
    private readonly HttpClient _http;

    public DesktopApiClient(HttpClient http) => _http = http;

    public async Task<ApiResult<IReadOnlyList<DesktopAppDto>>> GetAppsAsync(CancellationToken ct = default)
    {
        var r = await ApiCall.SendAsync(c => _http.GetFromJsonAsync<List<DesktopAppDto>>("/api/apps", c), ct);
        return r.Cast<IReadOnlyList<DesktopAppDto>>(list => list);
    }

    public Task<ApiResult<ContentDto>> GetWelcomeAsync(CancellationToken ct = default) =>
        ApiCall.SendAsync(c => _http.GetFromJsonAsync<ContentDto>("/api/content/welcome", c), ct);

    public Task<ApiResult<ContentDto>> GetAboutAsync(CancellationToken ct = default) =>
        ApiCall.SendAsync(c => _http.GetFromJsonAsync<ContentDto>("/api/content/about", c), ct);

    public Task<ApiResult<ContentDto>> GetReportAsync(CancellationToken ct = default) =>
        ApiCall.SendAsync(c => _http.GetFromJsonAsync<ContentDto>("/api/content/report", c), ct);

    public async Task<ApiResult<IReadOnlyList<NotificationDto>>> GetNotificationsAsync(CancellationToken ct = default)
    {
        var r = await ApiCall.SendAsync(c => _http.GetFromJsonAsync<List<NotificationDto>>("/api/notifications", c), ct);
        return r.Cast<IReadOnlyList<NotificationDto>>(list => list);
    }
}
