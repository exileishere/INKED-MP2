using INKED.Application.Exceptions;

namespace INKED.Application.Helpers;

/// <summary>Small shared helpers that mirror the MP1 SQL CHECK constraints.</summary>
public static class ValidationRules
{
    /// <summary>
    /// Matches a value against the allowed list (case-insensitive, like the SQL collation)
    /// and returns the canonical spelling that the CHECK constraint expects.
    /// </summary>
    public static string NormalizeChoice(string? value, string fieldName, IReadOnlyCollection<string> allowed)
    {
        var trimmed = value?.Trim();
        var match = allowed.FirstOrDefault(a => string.Equals(a, trimmed, StringComparison.OrdinalIgnoreCase));
        if (match is null)
        {
            var list = string.Join(", ", allowed.Select(a => $"'{a}'"));
            throw new BadRequestException($"{fieldName} must be one of: {list}.");
        }
        return match;
    }

    /// <summary>Validates a non-negative DECIMAL(precision, scale) value.</summary>
    public static void EnsureMoney(decimal value, string fieldName, int precision, int scale = 2)
    {
        if (value < 0)
            throw new BadRequestException($"{fieldName} cannot be negative.");

        if (decimal.Round(value, scale) != value)
            throw new BadRequestException($"{fieldName} cannot have more than {scale} decimal places.");

        decimal limit = 1m;
        for (var i = 0; i < precision - scale; i++) limit *= 10m;

        if (value >= limit)
            throw new BadRequestException($"{fieldName} must be less than {limit:N0}.");
    }

    /// <summary>DATETIME2(0) stores whole seconds, so values are truncated before they are compared or saved.</summary>
    public static DateTime ToSecond(DateTime value)
        => new(value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond), value.Kind);

    public static DateTime NowToSecond() => ToSecond(DateTime.Now);

    public static string? NullIfBlank(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
