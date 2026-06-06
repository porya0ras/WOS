using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Test/diagnostics calls used by the "API Tester" sample app to exercise the
/// framework's error handling. Returns <see cref="ApiResult{T}"/> like the
/// content service.
/// </summary>
public interface IDiagnosticsService
{
    /// <summary>Asks the API to respond with the given HTTP status code.</summary>
    Task<ApiResult<ContentDto>> RequestStatusAsync(int statusCode, CancellationToken ct = default);
}
