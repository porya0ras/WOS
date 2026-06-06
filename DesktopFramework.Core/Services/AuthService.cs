using DesktopFramework.Contracts;

namespace DesktopFramework.Core.Services;

/// <summary>Where the user is in the (up to) two-step login flow.</summary>
public enum AuthStage
{
    LoggedOut,
    AwaitingContext, // step 1 passed, waiting for company/role selection
    Authenticated,
}

/// <summary>
/// Drives the two-step login flow and holds the session. Step 1 authenticates
/// credentials; step 2 selects company/role. If <see cref="LoginFlowOptions.EnableContextStep"/>
/// is off (or there's nothing to choose), step 1 finalizes the session directly —
/// so a project can drop the second step entirely. The final session is persisted
/// to sessionStorage. Scoped per circuit; raises <see cref="Changed"/> on change.
/// </summary>
public sealed class AuthService
{
    private const string SessionKey = "wos.auth.session";

    private readonly IAuthApiClient _api;
    private readonly ISessionStore _store;

    public AuthService(IAuthApiClient api, ISessionStore store, LoginFlowOptions options)
    {
        _api = api;
        _store = store;
        Options = options;
    }

    public LoginFlowOptions Options { get; }

    public AuthStage Stage { get; private set; } = AuthStage.LoggedOut;
    public AuthChallengeDto? Challenge { get; private set; }
    public AuthSessionDto? Session { get; private set; }
    public UserDto? User => Session?.User;
    public bool IsAuthenticated => Session is not null;

    public event Action? Changed;

    /// <summary>Step 1. On success either advances to <see cref="AuthStage.AwaitingContext"/>
    /// or finalizes immediately when there's no context step.</summary>
    public async Task<ApiResult<bool>> AuthenticateAsync(string username, string password, CancellationToken ct = default)
    {
        var result = await _api.AuthenticateAsync(new CredentialsRequest { Username = username, Password = password }, ct);
        if (result is not { IsSuccess: true, Value: { } challenge })
            return result.Cast(_ => false);

        Challenge = challenge;

        // Is there actually a second step to show?
        var hasContext = Options.HasContextStep
            && ((Options.ShowCompanySelector && challenge.Companies.Count > 0)
                || (Options.ShowRoleSelector && challenge.Roles.Count > 0));

        if (!hasContext)
        {
            var finalized = await CompleteAsync(null, null, ct);
            return finalized.Cast(_ => true);
        }

        Stage = AuthStage.AwaitingContext;
        Changed?.Invoke();
        return ApiResult<bool>.Ok(true);
    }

    /// <summary>Step 2 (or the auto-finalize). Sets the session on success.</summary>
    public async Task<ApiResult<AuthSessionDto>> CompleteAsync(string? companyId, string? roleId, CancellationToken ct = default)
    {
        if (Challenge is null)
            return ApiResult<AuthSessionDto>.Fail(null, "Session expired", "Please sign in again.");

        var result = await _api.CompleteAsync(new CompleteLoginRequest
        {
            Ticket = Challenge.Ticket,
            CompanyId = companyId,
            RoleId = roleId,
        }, ct);

        if (result is { IsSuccess: true, Value: { } session })
        {
            Session = session;
            Challenge = null;
            Stage = AuthStage.Authenticated;
            await _store.SetAsync(SessionKey, session, ct);
            Changed?.Invoke();
        }
        return result;
    }

    /// <summary>Returns from step 2 to the credentials step.</summary>
    public void BackToCredentials()
    {
        Challenge = null;
        Stage = AuthStage.LoggedOut;
        Changed?.Invoke();
    }

    /// <summary>Restores a saved session from sessionStorage (call once after first render).</summary>
    public async Task RestoreAsync(CancellationToken ct = default)
    {
        Session = await _store.GetAsync<AuthSessionDto>(SessionKey, ct);
        if (Session is not null)
        {
            Stage = AuthStage.Authenticated;
            Changed?.Invoke();
        }
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        Session = null;
        Challenge = null;
        Stage = AuthStage.LoggedOut;
        await _store.RemoveAsync(SessionKey, ct);
        Changed?.Invoke();
    }

    public bool IsInRole(string role) =>
        User?.Roles.Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;
}
