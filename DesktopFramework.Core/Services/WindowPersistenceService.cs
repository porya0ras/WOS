using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Services;

/// <summary>One persisted window's layout (geometry + state). Parameters are not
/// persisted in v1 — restored windows open with their defaults (plan §16).</summary>
public sealed record PersistedWindow
{
    public required string AppId { get; init; }
    public string? InstanceKey { get; init; }
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public WindowState State { get; init; }
    public int ZIndex { get; init; }
}

/// <summary>
/// Saves the open-window layout (debounced) on every <see cref="WindowManager.Changed"/>
/// and restores it on load via <see cref="IDesktopPersistence"/>.
/// </summary>
public sealed class WindowPersistenceService : IDisposable
{
    private const string StorageKey = "wos.windows";
    private static readonly TimeSpan Debounce = TimeSpan.FromMilliseconds(500);

    private readonly WindowManager _windows;
    private readonly IDesktopPersistence _persistence;

    private CancellationTokenSource? _pending;
    private bool _suppress;

    public WindowPersistenceService(WindowManager windows, IDesktopPersistence persistence)
    {
        _windows = windows;
        _persistence = persistence;
        _windows.Changed += OnWindowsChanged;
    }

    public async Task RestoreAsync(CancellationToken ct = default)
    {
        var saved = await _persistence.GetAsync<List<PersistedWindow>>(StorageKey, ct);
        if (saved is null || saved.Count == 0) return;

        _suppress = true;
        try
        {
            foreach (var w in saved.OrderBy(w => w.ZIndex))
            {
                var opened = _windows.OpenApp(w.AppId, new()
                {
                    InstanceKey = w.InstanceKey,
                    WindowOverrides = new WindowOptions
                    {
                        InitialPosition = new WindowPosition(w.X, w.Y),
                        InitialSize = new WindowSize(w.Width, w.Height),
                        StartMaximized = w.State == WindowState.Maximized,
                    },
                });

                if (opened is not null && w.State == WindowState.Minimized)
                    _windows.Minimize(opened.Id);
            }
        }
        finally
        {
            _suppress = false;
        }
    }

    private void OnWindowsChanged()
    {
        if (_suppress) return;

        _pending?.Cancel();
        _pending?.Dispose();
        _pending = new CancellationTokenSource();
        var token = _pending.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Debounce, token);
                await SaveAsync(token);
            }
            catch (OperationCanceledException) { /* superseded by a newer change */ }
        }, token);
    }

    private Task SaveAsync(CancellationToken ct)
    {
        var snapshot = _windows.Windows.Select(w => new PersistedWindow
        {
            AppId = w.AppId,
            InstanceKey = w.InstanceKey,
            X = w.Position.X,
            Y = w.Position.Y,
            Width = w.Size.Width,
            Height = w.Size.Height,
            State = w.State,
            ZIndex = w.ZIndex,
        }).ToList();

        return _persistence.SetAsync(StorageKey, snapshot, ct);
    }

    public void Dispose()
    {
        _windows.Changed -= OnWindowsChanged;
        _pending?.Cancel();
        _pending?.Dispose();
    }
}
