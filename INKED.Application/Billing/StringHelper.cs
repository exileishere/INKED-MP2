// INKED MP2 | Shared helper: optional text fields are stored as NULL when blank
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
namespace INKED.Application.Billing;

public static class StringHelper
{
    public static string? ToNullIfBlank(this string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
