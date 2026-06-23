using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VolcanREG.Helpers;

public static partial class PlateValidator
{
    public static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().Replace(" ", string.Empty).ToUpperInvariant();
    }

    public static bool IsValidColombianPlate(string? value)
    {
        return ColombianPlateRegex().IsMatch(Normalize(value));
    }

    [GeneratedRegex("^[A-Z]{3}[0-9]{3}$")]
    private static partial Regex ColombianPlateRegex();
}

public sealed class ColombianPlateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return PlateValidator.IsValidColombianPlate(value?.ToString());
    }
}
