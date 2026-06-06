namespace DesktopFramework.Contracts;

/// <summary>The signed-in user, including the company/role they chose.</summary>
public sealed record UserDto
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }

    public string? CompanyId { get; init; }
    public string? CompanyName { get; init; }
    public string? RoleId { get; init; }
    public string? RoleName { get; init; }

    /// <summary>Effective roles used for authorization (e.g. ["Admin","Manager","User"]).</summary>
    public string[] Roles { get; init; } = [];
}

/// <summary>A login session: token + user. Stored client-side in sessionStorage.</summary>
public sealed record AuthSessionDto
{
    public required string Token { get; init; }
    public required UserDto User { get; init; }
    public DateTimeOffset IssuedAt { get; init; } = DateTimeOffset.UtcNow;
}
