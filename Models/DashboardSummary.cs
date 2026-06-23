namespace VolcanREG.Models;

public sealed class DashboardSummary
{
    public int TotalLoads { get; set; }
    public int PendingValidation { get; set; }
    public int Validated { get; set; }
    public int Incorrect { get; set; }
    public decimal ValidatedM3 { get; set; }
    public decimal PendingM3 { get; set; }
    public decimal IncorrectM3 { get; set; }
    public decimal ReviewedM3 { get; set; }
    public decimal IncorrectPercentage { get; set; }
    public decimal AverageValidatedM3 { get; set; }
    public DateTime? LastSyncUtc { get; set; }
}
