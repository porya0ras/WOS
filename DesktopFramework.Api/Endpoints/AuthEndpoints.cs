using DesktopFramework.Api.Services;
using DesktopFramework.Contracts;

namespace DesktopFramework.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        // Step 1: credentials -> challenge (ticket + selectable companies/roles).
        group.MapPost("/authenticate", (CredentialsRequest request, AuthProvider provider) =>
            provider.Authenticate(request) is { } challenge
                ? Results.Ok(challenge)
                : Results.Unauthorized())
             .WithName("Authenticate");

        // Step 2: ticket + chosen company/role -> session.
        group.MapPost("/complete", (CompleteLoginRequest request, AuthProvider provider) =>
            provider.Complete(request) is { } session
                ? Results.Ok(session)
                : Results.Unauthorized())
             .WithName("CompleteLogin");

        return app;
    }
}
