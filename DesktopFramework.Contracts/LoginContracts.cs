namespace DesktopFramework.Contracts;

public sealed record CompanyDto(string Id, string Name);

public sealed record RoleDto(string Id, string Name);

/// <summary>Step 1: the credentials submitted to /api/auth/authenticate.</summary>
public sealed record CredentialsRequest
{
    public string Username { get; init; } = "";
    public string Password { get; init; } = "";
}

/// <summary>
/// Step 1 result: the user is authenticated but the session isn't finalized yet.
/// <see cref="Ticket"/> carries the pending auth; Companies/Roles are what this user
/// may choose from in step 2. (Both can be empty — then there's nothing to pick.)
/// </summary>
public sealed record AuthChallengeDto
{
    public required string Ticket { get; init; }
    public IReadOnlyList<CompanyDto> Companies { get; init; } = [];
    public IReadOnlyList<RoleDto> Roles { get; init; } = [];
}

/// <summary>Step 2: finalize the login with the chosen company/role.</summary>
public sealed record CompleteLoginRequest
{
    public required string Ticket { get; init; }
    public string? CompanyId { get; init; }
    public string? RoleId { get; init; }
}
