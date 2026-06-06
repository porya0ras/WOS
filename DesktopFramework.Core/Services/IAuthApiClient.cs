using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>Talks to the auth API (two-step flow). Implemented in the Web project.</summary>
public interface IAuthApiClient
{
    /// <summary>Step 1 — validate credentials, returning a pending challenge.</summary>
    Task<ApiResult<AuthChallengeDto>> AuthenticateAsync(CredentialsRequest request, CancellationToken ct = default);

    /// <summary>Step 2 — finalize with the chosen company/role.</summary>
    Task<ApiResult<AuthSessionDto>> CompleteAsync(CompleteLoginRequest request, CancellationToken ct = default);
}
