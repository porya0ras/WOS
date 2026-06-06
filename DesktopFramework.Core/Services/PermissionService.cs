using DesktopFramework.Core.Apps;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Real permission service: an app with no <see cref="AppDescriptor.RequiredRoles"/>
/// is always allowed; otherwise the signed-in user must hold at least one of them.
/// Register this (over <see cref="AllowAllPermissionService"/>) to enable role gating.
/// </summary>
public sealed class PermissionService : IPermissionService
{
    private readonly AuthService _auth;

    public PermissionService(AuthService auth) => _auth = auth;

    public bool CanLaunch(AppDescriptor app)
    {
        if (app.RequiredRoles.Length == 0)
            return true;

        return app.RequiredRoles.Any(_auth.IsInRole);
    }
}
