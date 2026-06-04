using DesktopFramework.Api.Services;

namespace DesktopFramework.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/notifications", (NotificationProvider provider) => provider.GetNotifications())
           .WithName("GetNotifications");

        return app;
    }
}
