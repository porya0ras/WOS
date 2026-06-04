using DesktopFramework.Api.Services;

namespace DesktopFramework.Api.Endpoints;

public static class AppsEndpoints
{
    public static IEndpointRouteBuilder MapAppsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/apps", (AppContentProvider provider) => provider.GetApps())
           .WithName("GetApps");

        return app;
    }
}
