using VolcanREG.Security;

namespace VolcanREG.Models;

public sealed class ReportFilters
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? OperatorId { get; set; }
    public string? VehiclePlate { get; set; }
    public string? DriverCcOrName { get; set; }
    public string? CustomerName { get; set; }
    public string ValidationStatus { get; set; } = "All";

    public bool Matches(LoadRecord record)
    {
        if (StartDate is not null && record.LoadedAtLocal.Date < StartDate.Value.Date)
        {
            return false;
        }

        if (EndDate is not null && record.LoadedAtLocal.Date > EndDate.Value.Date)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(OperatorId) && record.OperatorId != OperatorId)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(VehiclePlate) &&
            !record.VehiclePlate.Contains(VehiclePlate, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(DriverCcOrName) &&
            !record.DriverCc.Contains(DriverCcOrName, StringComparison.OrdinalIgnoreCase) &&
            !record.DriverName.Contains(DriverCcOrName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(CustomerName) &&
            !(record.CustomerName?.Contains(CustomerName, StringComparison.OrdinalIgnoreCase) ?? false))
        {
            return false;
        }

        return ValidationStatus == "All" || record.ValidationStatus == ValidationStatus;
    }
}
