namespace DesktopFramework.Core.Services;

/// <summary>
/// Tracks whether the desktop is "busy" (e.g. loading from an API). The shell
/// reflects <see cref="IsBusy"/> by showing the OS wait cursor. Reference-counted
/// so concurrent/nested operations behave correctly. Scoped per circuit.
/// </summary>
public sealed class BusyService
{
    private int _count;

    public bool IsBusy => _count > 0;

    /// <summary>Raised when the busy state flips (idle↔busy).</summary>
    public event Action? Changed;

    /// <summary>Marks the desktop busy until the returned token is disposed.</summary>
    public IDisposable Begin()
    {
        if (++_count == 1)
            Changed?.Invoke();
        return new Token(this);
    }

    /// <summary>Runs <paramref name="action"/> while showing the busy cursor.</summary>
    public async Task RunAsync(Func<Task> action)
    {
        using (Begin())
            await action();
    }

    /// <summary>Runs <paramref name="action"/> while showing the busy cursor, returning its result.</summary>
    public async Task<T> RunAsync<T>(Func<Task<T>> action)
    {
        using (Begin())
            return await action();
    }

    private void End()
    {
        if (_count == 0) return;
        if (--_count == 0)
            Changed?.Invoke();
    }

    private sealed class Token(BusyService owner) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            owner.End();
        }
    }
}
