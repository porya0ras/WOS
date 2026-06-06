using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>
/// No-op <see cref="IDesktopContentService"/> used when the host hasn't registered one,
/// so the desktop runs without a backend. Lists come back empty; single items report
/// "not configured". Replace by registering your own API-backed implementation.
/// </summary>
public sealed class EmptyContentService : IDesktopContentService
{
    private static ApiResult<ContentDto> NotConfigured() =>
        ApiResult<ContentDto>.Fail(null, "No content service",
            "Register an IDesktopContentService that calls your API.");

    public Task<ApiResult<IReadOnlyList<DesktopAppDto>>> GetAppsAsync(CancellationToken ct = default) =>
        Task.FromResult(ApiResult<IReadOnlyList<DesktopAppDto>>.Ok(Array.Empty<DesktopAppDto>()));

    public Task<ApiResult<ContentDto>> GetWelcomeAsync(CancellationToken ct = default) => Task.FromResult(NotConfigured());
    public Task<ApiResult<ContentDto>> GetAboutAsync(CancellationToken ct = default) => Task.FromResult(NotConfigured());
    public Task<ApiResult<ContentDto>> GetReportAsync(CancellationToken ct = default) => Task.FromResult(NotConfigured());

    public Task<ApiResult<IReadOnlyList<NotificationDto>>> GetNotificationsAsync(CancellationToken ct = default) =>
        Task.FromResult(ApiResult<IReadOnlyList<NotificationDto>>.Ok(Array.Empty<NotificationDto>()));
}

/// <summary>
/// No-op <see cref="IAuthApiClient"/> used when the host hasn't registered one. Login
/// fails with a clear message until you wire up a real auth client.
/// </summary>
public sealed class OfflineAuthApiClient : IAuthApiClient
{
    public Task<ApiResult<AuthChallengeDto>> AuthenticateAsync(CredentialsRequest request, CancellationToken ct = default) =>
        Task.FromResult(ApiResult<AuthChallengeDto>.Fail(null, "Authentication not configured",
            "Register an IAuthApiClient that calls your API."));

    public Task<ApiResult<AuthSessionDto>> CompleteAsync(CompleteLoginRequest request, CancellationToken ct = default) =>
        Task.FromResult(ApiResult<AuthSessionDto>.Fail(null, "Authentication not configured",
            "Register an IAuthApiClient that calls your API."));
}
