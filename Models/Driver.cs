namespace VolcanREG.Models;

public sealed class Driver
{
    public string DriverCc { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string? LastVehiclePlate { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
