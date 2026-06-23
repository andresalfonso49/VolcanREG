namespace VolcanREG.Models;

public sealed class ValidationLog
{
    public string Id { get; set; } = string.Empty;
    public string LoadRecordId { get; set; } = string.Empty;
    public string PreviousValidationStatus { get; set; } = string.Empty;
    public string NewValidationStatus { get; set; } = string.Empty;
    public string ChangedByUserId { get; set; } = string.Empty;
    public string ChangedByUserName { get; set; } = string.Empty;
    public DateTime ChangedAtUtc { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
