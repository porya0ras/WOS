namespace DesktopFramework.Core.Services;

public enum DesktopTheme
{
    Light,
    Dark,
}

/// <summary>
/// Holds the current theme and persists the choice. The shell reflects
/// <see cref="Current"/> onto the root element's <c>data-theme</c> attribute,
/// which swaps the CSS-variable set (plan §19). Scoped per circuit.
/// </summary>
public sealed class ThemeService
{
    private const string StorageKey = "wos.theme";
    private readonly IDesktopPersistence _persistence;

    public ThemeService(IDesktopPersistence persistence) => _persistence = persistence;

    public DesktopTheme Current { get; private set; } = DesktopTheme.Light;

    public event Action? Changed;

    public string DataThemeValue => Current == DesktopTheme.Dark ? "dark" : "light";

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        var saved = await _persistence.GetAsync<string>(StorageKey, ct);
        if (Enum.TryParse<DesktopTheme>(saved, ignoreCase: true, out var theme))
        {
            Current = theme;
            Changed?.Invoke();
        }
    }

    public async Task SetThemeAsync(DesktopTheme theme, CancellationToken ct = default)
    {
        if (Current == theme) return;
        Current = theme;
        await _persistence.SetAsync(StorageKey, theme.ToString(), ct);
        Changed?.Invoke();
    }

    public Task ToggleDarkModeAsync(CancellationToken ct = default) =>
        SetThemeAsync(Current == DesktopTheme.Dark ? DesktopTheme.Light : DesktopTheme.Dark, ct);
}
