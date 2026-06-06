using System.Net.Http.Json;
using DesktopFramework.Contracts;
using DesktopFramework.Core;
using DesktopFramework.Core.Services;

namespace DesktopFramework.Web.Services;

/// <summary>Calls the two-step auth API. Errors come back as <see cref="ApiResult{T}"/>.</summary>
public sealed class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http) => _http = http;

    public async Task<ApiResult<AuthChallengeDto>> AuthenticateAsync(CredentialsRequest request, CancellationToken ct = default)
    {
        var result = await ApiCall.SendAsync(async c =>
        {
            var response = await _http.PostAsJsonAsync("/api/auth/authenticate", request, c);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthChallengeDto>(c);
        }, ct);

        return result is { IsSuccess: false, StatusCode: 401 }
            ? ApiResult<AuthChallengeDto>.Fail(401, "Sign in failed", "Invalid username or password.")
            : result;
    }

    public Task<ApiResult<AuthSessionDto>> CompleteAsync(CompleteLoginRequest request, CancellationToken ct = default) =>
        ApiCall.SendAsync(async c =>
        {
            var response = await _http.PostAsJsonAsync("/api/auth/complete", request, c);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthSessionDto>(c);
        }, ct);
}
