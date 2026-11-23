// INKED MP2 | Shared exception: request breaks a business rule or references a missing record (HTTP 400)
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
namespace INKED.Application.Billing;

public class ValidationFailedException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationFailedException(Dictionary<string, string[]> errors)
        : base("The request breaks one or more INKED business rules.")
    {
        Errors = errors;
    }

    public ValidationFailedException(string field, string message)
        : this(new Dictionary<string, string[]> { [field] = new[] { message } })
    {
    }
}
