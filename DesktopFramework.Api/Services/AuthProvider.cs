using System.Collections.Concurrent;
using DesktopFramework.Contracts;

namespace DesktopFramework.Api.Services;

/// <summary>
/// In-memory two-step authentication for v1. Step 1 (<see cref="Authenticate"/>)
/// validates credentials and issues a short-lived ticket plus the companies/roles
/// the user may choose from. Step 2 (<see cref="Complete"/>) redeems the ticket with
/// the chosen company/role and returns the session. Demo policy: any non-empty
/// username + password is accepted.
/// </summary>
public sealed class AuthProvider
{
    private readonly IReadOnlyList<CompanyDto> _companies =
    [
        new("acme", "Acme Corporation"),
        new("globex", "Globex"),
        new("initech", "Initech"),
    ];

    private readonly IReadOnlyList<RoleDto> _roles =
    [
        new("admin", "Administrator"),
        new("manager", "Manager"),
        new("user", "User"),
    ];

    // ticket -> username (pending logins awaiting the context step).
    private readonly ConcurrentDictionary<string, string> _pending = new();

    public AuthChallengeDto? Authenticate(CredentialsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var ticket = Guid.NewGuid().ToString("N");
        _pending[ticket] = request.Username.Trim();

        // In a real app, scope these to what the user is allowed.
        return new AuthChallengeDto
        {
            Ticket = ticket,
            Companies = _companies,
            Roles = _roles,
        };
    }

    public AuthSessionDto? Complete(CompleteLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Ticket) || !_pending.TryRemove(request.Ticket, out var username))
            return null;

        var company = _companies.FirstOrDefault(c => c.Id == request.CompanyId);
        var role = _roles.FirstOrDefault(r => r.Id == request.RoleId) ?? _roles[^1]; // default: User

        var roles = role.Id switch
        {
            "admin" => new[] { "Admin", "Manager", "User" },
            "manager" => new[] { "Manager", "User" },
            _ => new[] { "User" },
        };

        var user = new UserDto
        {
            Id = Guid.NewGuid().ToString("N"),
            Username = username,
            DisplayName = char.ToUpper(username[0]) + username[1..],
            CompanyId = company?.Id,
            CompanyName = company?.Name,
            RoleId = role.Id,
            RoleName = role.Name,
            Roles = roles,
        };

        return new AuthSessionDto { Token = Guid.NewGuid().ToString("N"), User = user };
    }
}
