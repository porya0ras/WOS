namespace DesktopFramework.Core.Services;

/// <summary>
/// Session-scoped key/value store (cleared when the browser tab closes).
/// Backed by sessionStorage in the Web project; used for the auth session.
/// </summary>
public interface ISessionStore
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
