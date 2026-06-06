using DesktopFramework.Core.Windowing;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Tracks the grid cell of each desktop icon so icons stay aligned to a regular
/// tile grid (Windows-style) while being draggable. Auto-assigns cells for new apps,
/// keeps icons from overlapping, and persists the layout (debounced) via
/// <see cref="IDesktopPersistence"/>. Scoped per circuit.
/// </summary>
public sealed class DesktopIconLayout
{
    private const string StorageKey = "wos.icons";
    private static readonly TimeSpan Debounce = TimeSpan.FromMilliseconds(400);

    private readonly IDesktopPersistence _persistence;
    private readonly Dictionary<string, GridCell> _cells = new(StringComparer.OrdinalIgnoreCase);
    private CancellationTokenSource? _saveCts;

    public DesktopIconLayout(IDesktopPersistence persistence) => _persistence = persistence;

    /// <summary>How many icons stack vertically before auto-assign moves to the next column.</summary>
    public int RowsPerColumn { get; set; } = 6;

    public event Action? Changed;

    public GridCell GetCell(string appId) =>
        _cells.TryGetValue(appId, out var cell) ? cell : default;

    /// <summary>Gives every listed app a cell (keeping existing ones). Call when the app list changes.</summary>
    public void EnsureAssigned(IEnumerable<string> appIds)
    {
        var changed = false;
        foreach (var id in appIds)
        {
            if (!_cells.ContainsKey(id))
            {
                _cells[id] = NextFreeCell();
                changed = true;
            }
        }
        if (changed)
        {
            Changed?.Invoke();
            QueueSave();
        }
    }

    /// <summary>Moves an icon to a grid cell, snapping to the nearest free cell if taken.</summary>
    public void Move(string appId, GridCell target)
    {
        target = new GridCell(Math.Max(0, target.Col), Math.Max(0, target.Row));
        if (IsOccupied(target, appId))
            target = NearestFreeCell(target, appId);

        if (_cells.TryGetValue(appId, out var current) && current.Equals(target))
            return;

        _cells[appId] = target;
        Changed?.Invoke();
        QueueSave();
    }

    public async Task RestoreAsync(CancellationToken ct = default)
    {
        var saved = await _persistence.GetAsync<Dictionary<string, GridCell>>(StorageKey, ct);
        if (saved is not { Count: > 0 }) return;

        _cells.Clear();
        foreach (var kvp in saved)
            _cells[kvp.Key] = kvp.Value;
        Changed?.Invoke();
    }

    private bool IsOccupied(GridCell cell, string exceptApp) =>
        _cells.Any(kvp => kvp.Key != exceptApp && kvp.Value.Equals(cell));

    private GridCell NextFreeCell()
    {
        for (var col = 0; col < 1000; col++)
            for (var row = 0; row < RowsPerColumn; row++)
            {
                var cell = new GridCell(col, row);
                if (!_cells.ContainsValue(cell)) return cell;
            }
        return new GridCell(0, 0);
    }

    private GridCell NearestFreeCell(GridCell from, string exceptApp)
    {
        for (var radius = 1; radius < 100; radius++)
            for (var dc = -radius; dc <= radius; dc++)
                for (var dr = -radius; dr <= radius; dr++)
                {
                    var cell = new GridCell(from.Col + dc, from.Row + dr);
                    if (cell.Col < 0 || cell.Row < 0) continue;
                    if (!IsOccupied(cell, exceptApp)) return cell;
                }
        return from;
    }

    private void QueueSave()
    {
        _saveCts?.Cancel();
        _saveCts?.Dispose();
        _saveCts = new CancellationTokenSource();
        var token = _saveCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Debounce, token);
                await _persistence.SetAsync(StorageKey, _cells, token);
            }
            catch (OperationCanceledException) { }
        }, token);
    }
}
