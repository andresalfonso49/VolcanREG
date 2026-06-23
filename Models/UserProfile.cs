using VolcanREG.Security;

namespace VolcanREG.Models;

public sealed class UserProfile
{
    public string Uid { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = AppRoles.Operator;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public bool IsAdmin => Role == AppRoles.Admin;
    public bool IsOperator => Role == AppRoles.Operator;
}
