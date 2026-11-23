// INKED MP2 | Shared helper that collects field errors before throwing ValidationFailedException
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
namespace INKED.Application.Billing;

public class ValidationErrors
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool IsEmpty => _errors.Count == 0;

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var list))
        {
            list = new List<string>();
            _errors[field] = list;
        }
        list.Add(message);
    }

    public void ThrowIfAny()
    {
        if (IsEmpty) return;
        throw new ValidationFailedException(_errors.ToDictionary(e => e.Key, e => e.Value.ToArray()));
    }
}
