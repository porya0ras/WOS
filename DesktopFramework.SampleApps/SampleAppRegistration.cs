using DesktopFramework.Core.Apps;
using DesktopFramework.Core.Services;
using DesktopFramework.Core.Windowing;

namespace DesktopFramework.SampleApps;

/// <summary>
/// Registers the demo apps, mapping each stable id to the Blazor component that
/// renders inside its window. This is the local source of <c>ComponentType</c>
/// that the API catalog (which carries only ids) cannot provide (plan §10).
/// </summary>
public static class SampleAppRegistration
{
    public static void RegisterSampleApps(this AppRegistry registry)
    {
        registry.RegisterAll(
        [
            new AppDescriptor
            {
                Id = "welcome",
                Title = "Welcome",
                Icon = "fa-solid fa-hands-clapping",
                ComponentType = typeof(WelcomeApp),
                Category = DesktopAppCategory.System,
                Order = 0,
                DefaultWindowOptions = new WindowOptions { InitialSize = new(560, 380) },
            },
            new AppDescriptor
            {
                Id = "about",
                Title = "About",
                Icon = "fa-solid fa-circle-info",
                ComponentType = typeof(AboutApp),
                Category = DesktopAppCategory.System,
                Order = 1,
                DefaultWindowOptions = new WindowOptions { InitialSize = new(520, 360) },
            },
            new AppDescriptor
            {
                Id = "settings",
                Title = "Settings",
                Icon = "fa-solid fa-gear",
                ComponentType = typeof(SettingsApp),
                Category = DesktopAppCategory.System,
                Order = 2,
                AllowMultipleInstances = false, // single-instance demo
                DefaultWindowOptions = new WindowOptions { InitialSize = new(440, 360) },
            },
            new AppDescriptor
            {
                Id = "customers",
                Title = "Customers",
                Icon = "fa-solid fa-user",
                ComponentType = typeof(CustomerApp),
                Category = DesktopAppCategory.Productivity,
                Order = 3,
                AllowMultipleInstances = true, // multi-instance demo
                DefaultWindowOptions = new WindowOptions { InitialSize = new(480, 340) },
            },
        ]);
    }
}
