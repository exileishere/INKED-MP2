// INKED MP2 | Shared DTO validation: decimal must not have more decimal places than the SQL column allows
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Billing;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DecimalPlacesAttribute : ValidationAttribute
{
    public int Places { get; }

    public DecimalPlacesAttribute(int places)
    {
        Places = places;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not decimal d) return ValidationResult.Success;

        if (decimal.Round(d, Places) == d) return ValidationResult.Success;

        return new ValidationResult(
            $"{validationContext.DisplayName} can have at most {Places} decimal places.",
            new[] { validationContext.MemberName ?? validationContext.DisplayName });
    }
}
