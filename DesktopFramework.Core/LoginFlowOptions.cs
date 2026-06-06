namespace DesktopFramework.Core;

/// <summary>
/// Configures the login flow so it can be reused across projects. The login is a
/// two-step flow by default (credentials → company/role). To make it a plain
/// single-step login, set <see cref="EnableContextStep"/> to <c>false</c> — the
/// second step is then never shown and the session is finalized right after the
/// credentials step. Each selector can also be toggled/required independently.
/// </summary>
public sealed class LoginFlowOptions
{
    /// <summary>Show step 2 (company/role selection). Set false to drop it entirely.</summary>
    public bool EnableContextStep { get; set; } = true;

    public bool ShowCompanySelector { get; set; } = true;
    public bool ShowRoleSelector { get; set; } = true;

    public bool RequireCompany { get; set; }
    public bool RequireRole { get; set; }

    // UI text (override per project / for localization).
    public string CredentialsTitle { get; set; } = "Sign in";
    public string CredentialsSubtitle { get; set; } = "Sign in to your web desktop.";
    public string ContextTitle { get; set; } = "Choose your workspace";
    public string ContextSubtitle { get; set; } = "Select the company and role for this session.";
    public string CompanyLabel { get; set; } = "Company";
    public string RoleLabel { get; set; } = "Role";

    /// <summary>True when step 2 should run at all.</summary>
    public bool HasContextStep => EnableContextStep && (ShowCompanySelector || ShowRoleSelector);
}
