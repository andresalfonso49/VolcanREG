namespace VolcanREG.Models;

public sealed class Vehicle
{
    public string VehiclePlate { get; set; } = string.Empty;
    public string? LastDriverCc { get; set; }
    public string? LastDriverName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
