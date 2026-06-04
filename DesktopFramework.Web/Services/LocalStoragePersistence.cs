using System.Text.Json;
using DesktopFramework.Core.Services;
using Microsoft.JSInterop;

namespace DesktopFramework.Web.Services;

/// <summary>
/// v1 persistence backed by browser localStorage via the framework's JS module.
/// Implements the <see cref="IDesktopPersistence"/> seam so a future Aspire
/// backend can replace it with no UI changes (plan §16).
/// </summary>
public sealed class LocalStoragePersistence : IDesktopPersistence, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public LocalStoragePersistence(IJSRuntime js) => _js = js;

    private async ValueTask<IJSObjectReference> ModuleAsync() =>
        _module ??= await _js.InvokeAsync<IJSObjectReference>(
            "import", "./_content/DesktopFramework.Components/js/window-interop.js");

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var module = await ModuleAsync();
        var json = await module.InvokeAsync<string?>("storageGet", ct, key);
        if (string.IsNullOrEmpty(json)) return default;

        try { return JsonSerializer.Deserialize<T>(json, JsonOptions); }
        catch (JsonException) { return default; }
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken ct = default)
    {
        var module = await ModuleAsync();
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await module.InvokeVoidAsync("storageSet", ct, key, json);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("storageRemove", ct, key);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module is not null) await _module.DisposeAsync();
        }
        catch (JSDisconnectedException) { }
    }
}
