using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Reads desktop content from the backend API. Implemented in the Web project.
/// All calls return <see cref="ApiResult{T}"/> — failures are data, never thrown,
/// so the UI can show a friendly error instead of crashing.
/// </summary>
public interface IDesktopContentService
{
    Task<ApiResult<IReadOnlyList<DesktopAppDto>>> GetAppsAsync(CancellationToken ct = default);
    Task<ApiResult<ContentDto>> GetWelcomeAsync(CancellationToken ct = default);
    Task<ApiResult<ContentDto>> GetAboutAsync(CancellationToken ct = default);

    /// <summary>Calls the slow report endpoint (used to demo the loading cursor).</summary>
    Task<ApiResult<ContentDto>> GetReportAsync(CancellationToken ct = default);

    Task<ApiResult<IReadOnlyList<NotificationDto>>> GetNotificationsAsync(CancellationToken ct = default);
}
