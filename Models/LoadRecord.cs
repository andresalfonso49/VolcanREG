using System.ComponentModel.DataAnnotations;
using VolcanREG.Helpers;
using VolcanREG.Security;

namespace VolcanREG.Models;

public sealed class LoadRecord
{
    public string LocalId { get; set; } = GuidHelper.NewId();
    public string ClientRecordId { get; set; } = GuidHelper.NewId();
    public string? ServerRecordId { get; set; }
    public DateTime LoadedAtLocal { get; set; } = DateTimeHelper.LocalNow();
    public DateTime LoadedAtUtc { get; set; } = DateTimeHelper.UtcNow();
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorNameSnapshot { get; set; } = string.Empty;
    public string Material { get; set; } = "Arena";

    [Required(ErrorMessage = "La cantidad en m3 es obligatoria.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
    public decimal VolumeM3 { get; set; }

    [Required(ErrorMessage = "La placa es obligatoria.")]
    [ColombianPlate(ErrorMessage = "La placa debe tener formato ABC123.")]
    public string VehiclePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del conductor es obligatorio.")]
    public string DriverName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cedula es obligatoria.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "La cedula debe ser numerica.")]
    [MinLength(6, ErrorMessage = "La cedula debe tener minimo 6 digitos.")]
    public string DriverCc { get; set; } = string.Empty;

    public string? CustomerName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PhotoBase64 { get; set; }
    public string? Observations { get; set; }
    public string SyncStatus { get; set; } = SyncStatuses.Pending;
    public string ValidationStatus { get; set; } = ValidationStatuses.NotValidated;
    public string? ValidatedByUserId { get; set; }
    public string? ValidatedByUserName { get; set; }
    public DateTime? ValidatedAtUtc { get; set; }
    public string? ValidationNotes { get; set; }
    public string? IncorrectReason { get; set; }
    public DateTime CreatedAtLocal { get; set; } = DateTimeHelper.LocalNow();
    public DateTime CreatedAtUtc { get; set; } = DateTimeHelper.UtcNow();
    public DateTime? UpdatedAtLocal { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? SyncedAtUtc { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string? LastSyncError { get; set; }
}
