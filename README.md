# WOS — Blazor Web Desktop Framework on .NET Aspire

A reusable, browser-based "web desktop" shell built with Blazor (Interactive Server),
.NET 10 and .NET Aspire. Any Blazor component can be registered as an **app** and opened
inside a movable, resizable, minimizable/maximizable window. Multiple windows, a taskbar,
a start menu, theming and localStorage layout persistence are included.

## Run

```bash
dotnet run --project DesktopFramework.AppHost
```

This launches the Aspire dashboard and both resources (`web` + `api`). Open the **web**
endpoint shown in the dashboard. The desktop loads its welcome content and notifications
from `DesktopFramework.Api` via Aspire **service discovery** (logical name `api`, no
hardcoded URLs).

## Projects

| Project | Responsibility |
|---|---|
| `DesktopFramework.AppHost` | Aspire orchestration of `web` + `api`. |
| `DesktopFramework.ServiceDefaults` | Telemetry, health, service discovery, HttpClient resilience. |
| `DesktopFramework.Contracts` | DTOs shared by API + Web (`DesktopAppDto`, `ContentDto`, `NotificationDto`). |
| `DesktopFramework.Api` | In-memory content API: `/api/apps`, `/api/content/{welcome,about}`, `/api/notifications`. |
| `DesktopFramework.Core` | Windowing models + services (`WindowManager`, `AppRegistry`, `ZIndexManager`, `ThemeService`, persistence). No Razor. |
| `DesktopFramework.Components` | Razor Class Library: the desktop UI (shell, windows, taskbar, start menu) + JS interop + theme CSS. |
| `DesktopFramework.SampleApps` | Demo apps (Welcome, About, Settings = single-instance, Customers = multi-instance). |
| `DesktopFramework.Web` | Blazor host: `DesktopApiClient`, `LocalStoragePersistence`, DI wiring, registers the apps. |

## Registering an app

```csharp
registry.Register(new AppDescriptor
{
    Id = "customers",
    Title = "Customers",
    Icon = "👤",
    ComponentType = typeof(CustomerApp),
    AllowMultipleInstances = true,
});

// Open it (optionally with parameters / an instance key):
windowManager.OpenApp("customers", new AppLaunchOptions
{
    InstanceKey = "42",
    Parameters = new Dictionary<string, object?> { ["CustomerId"] = 42 },
});
```

The component is rendered inside its window via `DynamicComponent`. Drag/resize is handled
by a JS module (`window-interop.js`) for smooth, server-round-trip-free gestures; final
geometry is committed back to `WindowManager` on gesture end.

See `.claude/plans/` for the full technical plan and phase breakdown.
