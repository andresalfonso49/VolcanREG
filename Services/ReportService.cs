using VolcanREG.Models;
using VolcanREG.Security;

namespace VolcanREG.Services;

public sealed class ReportService
{
    public IReadOnlyList<LoadRecord> ApplyFilters(IEnumerable<LoadRecord> records, ReportFilters filters)
    {
        return records.Where(filters.Matches)
            .OrderByDescending(x => x.LoadedAtLocal)
            .ToArray();
    }

    public DashboardSummary BuildSummary(IEnumerable<LoadRecord> records)
    {
        var list = records.ToArray();
        var validated = list.Where(x => x.ValidationStatus == ValidationStatuses.Validated).ToArray();
        var pending = list.Where(x => x.ValidationStatus == ValidationStatuses.NotValidated).ToArray();
        var incorrect = list.Where(x => x.ValidationStatus == ValidationStatuses.Incorrect).ToArray();

        return new DashboardSummary
        {
            TotalLoads = list.Length,
            PendingValidation = pending.Length,
            Validated = validated.Length,
            Incorrect = incorrect.Length,
            ValidatedM3 = validated.Sum(x => x.VolumeM3),
            PendingM3 = pending.Sum(x => x.VolumeM3),
            IncorrectM3 = incorrect.Sum(x => x.VolumeM3),
            ReviewedM3 = validated.Sum(x => x.VolumeM3) + incorrect.Sum(x => x.VolumeM3),
            IncorrectPercentage = list.Length == 0 ? 0 : Math.Round((decimal)incorrect.Length / list.Length * 100, 2),
            AverageValidatedM3 = validated.Length == 0 ? 0 : Math.Round(validated.Average(x => x.VolumeM3), 2),
            LastSyncUtc = list.MaxBy(x => x.SyncedAtUtc)?.SyncedAtUtc
        };
    }

    public IReadOnlyDictionary<string, decimal> VolumeByOperator(IEnumerable<LoadRecord> records)
    {
        return records.GroupBy(x => string.IsNullOrWhiteSpace(x.OperatorNameSnapshot) ? "Sin operador" : x.OperatorNameSnapshot)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.VolumeM3));
    }

    public IReadOnlyDictionary<string, int> LoadsByStatus(IEnumerable<LoadRecord> records)
    {
        return records.GroupBy(x => x.ValidationStatus)
            .ToDictionary(x => x.Key, x => x.Count());
    }

    public IReadOnlyDictionary<string, decimal> VolumeByDate(IEnumerable<LoadRecord> records)
    {
        return records
            .GroupBy(x => x.LoadedAtLocal.Date)
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key.ToString("dd/MM"), x => x.Sum(y => y.VolumeM3));
    }

    public IReadOnlyDictionary<string, int> LoadsByDate(IEnumerable<LoadRecord> records)
    {
        return records
            .GroupBy(x => x.LoadedAtLocal.Date)
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key.ToString("dd/MM"), x => x.Count());
    }

    public IReadOnlyDictionary<string, decimal> VolumeByValidationStatus(IEnumerable<LoadRecord> records)
    {
        return records.GroupBy(x => x.ValidationStatus)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.VolumeM3));
    }
}
