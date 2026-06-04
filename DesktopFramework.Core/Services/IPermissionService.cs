using DesktopFramework.Core.Apps;

namespace DesktopFramework.Core.Services;

/// <summary>
/// Decides whether the current user may launch/see an app. v1 default allows all;
/// a real implementation checks the ClaimsPrincipal against the descriptor's
/// RequiredRoles/Claims/Permissions (plan §18).
/// </summary>
public interface IPermissionService
{
    bool CanLaunch(AppDescriptor app);
}

/// <summary>Default v1 implementation — everything is permitted.</summary>
public sealed class AllowAllPermissionService : IPermissionService
{
    public bool CanLaunch(AppDescriptor app) => true;
}
