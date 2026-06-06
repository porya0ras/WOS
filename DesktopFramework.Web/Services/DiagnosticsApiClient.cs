using System.Net.Http.Json;
using DesktopFramework.Contracts;
using DesktopFramework.Core;
using DesktopFramework.Core.Services;

namespace DesktopFramework.Web.Services;

/// <summary>Calls the API's /debug endpoints to exercise error handling.</summary>
public sealed class DiagnosticsApiClient : IDiagnosticsService
{
    private readonly HttpClient _http;

    public DiagnosticsApiClient(HttpClient http) => _http = http;

    public Task<ApiResult<ContentDto>> RequestStatusAsync(int statusCode, CancellationToken ct = default) =>
        ApiCall.SendAsync(c => _http.GetFromJsonAsync<ContentDto>($"/api/debug/status/{statusCode}", c), ct);
}
