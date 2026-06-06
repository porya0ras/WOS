using DesktopFramework.Api.Services;

namespace DesktopFramework.Api.Endpoints;

public static class ContentEndpoints
{
    public static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/content");

        group.MapGet("/welcome", (AppContentProvider provider) =>
            provider.GetContent("welcome") is { } c ? Results.Ok(c) : Results.NotFound())
            .WithName("GetWelcomeContent");

        group.MapGet("/about", (AppContentProvider provider) =>
            provider.GetContent("about") is { } c ? Results.Ok(c) : Results.NotFound())
            .WithName("GetAboutContent");

        // Deliberately slow endpoint so the desktop's loading cursor is visible.
        group.MapGet("/report", async (AppContentProvider provider, CancellationToken ct) =>
        {
            await Task.Delay(2500, ct);
            return Results.Ok(provider.GetReport());
        })
        .WithName("GetReport");

        return app;
    }
}
