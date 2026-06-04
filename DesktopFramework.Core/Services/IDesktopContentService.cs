using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>Reads desktop content from the backend API. Implemented in the Web project.</summary>
public interface IDesktopContentService
{
    Task<IReadOnlyList<DesktopAppDto>> GetAppsAsync(CancellationToken ct = default);
    Task<ContentDto?> GetWelcomeAsync(CancellationToken ct = default);
    Task<ContentDto?> GetAboutAsync(CancellationToken ct = default);
    Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(CancellationToken ct = default);
}
