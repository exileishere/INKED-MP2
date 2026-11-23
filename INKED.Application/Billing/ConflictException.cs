// INKED MP2 | Shared exception: request conflicts with existing data (HTTP 409)
// e.g. duplicate unique value, locked order, record still referenced by other tables.
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
namespace INKED.Application.Billing;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
