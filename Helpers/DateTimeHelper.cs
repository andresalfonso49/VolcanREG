namespace VolcanREG.Helpers;

public static class DateTimeHelper
{
    public static DateTime LocalNow() => DateTime.Now;

    public static DateTime UtcNow() => DateTime.UtcNow;

    public static string ToInputDate(DateTime value) => value.ToString("yyyy-MM-dd");
}
