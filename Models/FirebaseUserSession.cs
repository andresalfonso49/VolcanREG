namespace VolcanREG.Models;

public sealed class FirebaseUserSession
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? IdToken { get; set; }
}
