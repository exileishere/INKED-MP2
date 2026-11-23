// INKED MP2 | Shared DTO validation: value must be one of the values allowed by the SQL CHECK constraint
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Billing;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class OneOfAttribute : ValidationAttribute
{
    public string[] AllowedValues { get; }

    public OneOfAttribute(params string[] allowedValues)
    {
        AllowedValues = allowedValues;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Null is handled by [Required] (or by the database default for optional fields).
        if (value is null) return ValidationResult.Success;

        var text = value.ToString();
        if (text is not null && AllowedValues.Contains(text.Trim(), StringComparer.Ordinal))
            return ValidationResult.Success;

        return new ValidationResult(
            $"{validationContext.DisplayName} must be one of: {string.Join(", ", AllowedValues)}.",
            new[] { validationContext.MemberName ?? validationContext.DisplayName });
    }
}
