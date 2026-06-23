namespace VolcanREG.Models;

public sealed class SyncLog
{
    public string ClientRecordId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime AttemptedAtUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
