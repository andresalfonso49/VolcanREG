namespace VolcanREG.Models;

public sealed class EditLog
{
    public string Id { get; set; } = string.Empty;
    public string LoadRecordId { get; set; } = string.Empty;
    public string EditedByUserId { get; set; } = string.Empty;
    public string EditedByUserName { get; set; } = string.Empty;
    public DateTime EditedAtUtc { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }
}
