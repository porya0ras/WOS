using DesktopFramework.Contracts;

namespace DesktopFramework.Api.Endpoints;

/// <summary>Endpoints used by the "API Tester" sample app to exercise error handling.</summary>
public static class DebugEndpoints
{
    public static IEndpointRouteBuilder MapDebugEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/debug");

        // Responds with the requested HTTP status code (2xx returns a body).
        group.MapGet("/status/{code:int}", (int code) =>
            code is >= 200 and < 300
                ? Results.Ok(new ContentDto
                {
                    Key = "debug",
                    Title = $"Success ({code})",
                    Body = "The server returned a successful response.",
                    UpdatedAt = DateTimeOffset.Now,
                })
                : Results.StatusCode(code))
            .WithName("DebugStatus");

        return app;
    }
}
