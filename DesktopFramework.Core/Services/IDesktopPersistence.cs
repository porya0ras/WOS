namespace DesktopFramework.Core.Services;

/// <summary>
/// Key/value persistence seam. v1 impl writes to browser localStorage;
/// a future impl can call an Aspire backend service with no UI changes (plan §16).
/// </summary>
public interface IDesktopPersistence
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
