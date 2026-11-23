// INKED MP2 | Table: Payments | Layer: Application / Service Implementation
// Owner: Rashel Maxine Baquirin (Billing module)
//
// Business rules (from the MP1 database design, Tables 9, 11 and 12):
//  1. The order must exist and must already have an invoice that is not Cancelled
//     (the customer checks and confirms the invoice before paying).
//  2. Amount > 0 (CK_Payments_Amount); PaymentType and PaymentMethod must match the CHECK lists.
//  3. One payment per PaymentType per order (UQ_Payments_OrderID_PaymentType).
//  4. An order is paid either as Downpayment + Balance, or as one Full payment:
//       - Full cannot be combined with Downpayment or Balance;
//       - Balance can only be recorded after the Downpayment.
//  5. Downpayment must be at least the invoice's DownpaymentAmount (60% by default).
//     A Full payment must equal the invoice TotalAmount.
//  6. The total of all payments can never exceed the invoice TotalAmount.
//  7. Recording a Downpayment or Full payment locks the order (Orders.IsLocked = 1) in the same transaction.
//  8. A Downpayment cannot be deleted while a Balance payment still exists. Deleting a payment
//     does not unlock the order.
using INKED.Application.Billing;

namespace INKED.Application.Payments;

public class PaymentService : IPaymentService
{
    private const string Downpayment = "Downpayment";
    private const string Balance = "Balance";
    private const string Full = "Full";

    private readonly IPaymentRepository _repository;

    public PaymentService(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public Task<List<PaymentResponseDto>> GetAllAsync() => _repository.GetAllAsync();

    public Task<PaymentResponseDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto)
    {
        var payment = new PaymentResponseDto
        {
            OrderId = dto.OrderId!.Value,
            PaymentDate = dto.PaymentDate ?? DateTimeHelper.NowToSecond(),
            Amount = dto.Amount!.Value,
            PaymentType = dto.PaymentType.Trim(),
            PaymentMethod = dto.PaymentMethod.Trim(),
            ReferenceNo = dto.ReferenceNo.ToNullIfBlank()
        };

        if (!await _repository.OrderExistsAsync(payment.OrderId))
            throw new ValidationFailedException(nameof(payment.OrderId), $"Order with ID {payment.OrderId} does not exist.");

        await ValidatePaymentRulesAsync(payment, null);

        return await _repository.CreateAsync(payment, LocksOrder(payment.PaymentType));
    }

    public async Task<bool> UpdateAsync(int id, UpdatePaymentDto dto)
    {
        var payment = await _repository.GetByIdAsync(id);
        if (payment is null) return false;

        payment.PaymentDate = dto.PaymentDate!.Value;
        payment.Amount = dto.Amount!.Value;
        payment.PaymentType = dto.PaymentType.Trim();
        payment.PaymentMethod = dto.PaymentMethod.Trim();
        payment.ReferenceNo = dto.ReferenceNo.ToNullIfBlank();

        await ValidatePaymentRulesAsync(payment, id);

        return await _repository.UpdateAsync(payment, LocksOrder(payment.PaymentType));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _repository.GetByIdAsync(id);
        if (payment is null) return false;

        if (payment.PaymentType == Downpayment)
        {
            var otherTypes = await _repository.GetPaymentTypesForOrderAsync(payment.OrderId, id);
            if (otherTypes.Contains(Balance))
                throw new ConflictException(
                    $"Order {payment.OrderId} still has a Balance payment. Delete the Balance payment before the Downpayment.");
        }

        return await _repository.DeleteAsync(id);
    }

    private async Task ValidatePaymentRulesAsync(PaymentResponseDto payment, int? currentId)
    {
        // Rule 1: invoice must exist and not be cancelled.
        var invoice = await _repository.GetInvoiceForOrderAsync(payment.OrderId);
        if (invoice is null)
            throw new ConflictException($"Order {payment.OrderId} has no invoice yet. Issue the invoice before recording payments.");
        if (invoice.Status == "Cancelled")
            throw new ConflictException($"The invoice for order {payment.OrderId} is Cancelled, so no payment can be recorded.");

        // Rule 3: one payment per type per order.
        if (await _repository.PaymentTypeExistsForOrderAsync(payment.OrderId, payment.PaymentType, currentId))
            throw new ConflictException($"Order {payment.OrderId} already has a {payment.PaymentType} payment.");

        // Rule 4: valid combination of payment types.
        var otherTypes = await _repository.GetPaymentTypesForOrderAsync(payment.OrderId, currentId);
        if (payment.PaymentType == Full && otherTypes.Count > 0)
            throw new ConflictException(
                $"Order {payment.OrderId} is already being paid in parts ({string.Join(", ", otherTypes)}). A Full payment cannot be added.");
        if (payment.PaymentType != Full && otherTypes.Contains(Full))
            throw new ConflictException($"Order {payment.OrderId} was already paid in full.");
        if (payment.PaymentType == Balance && !otherTypes.Contains(Downpayment))
            throw new ConflictException($"Record the Downpayment for order {payment.OrderId} before the Balance payment.");

        // Rules 5 and 6: amounts compared with the invoice.
        var errors = new ValidationErrors();
        var downpaymentDue = invoice.DownpaymentAmount ?? Math.Round(invoice.TotalAmount * invoice.DownpaymentRate, 2);
        var paidByOthers = await _repository.GetTotalPaidForOrderAsync(payment.OrderId, currentId);
        var remaining = invoice.TotalAmount - paidByOthers;

        if (payment.PaymentType == Downpayment && payment.Amount < downpaymentDue)
            errors.Add(nameof(payment.Amount),
                $"The downpayment must be at least {downpaymentDue:N2} ({invoice.DownpaymentRate:P0} of {invoice.TotalAmount:N2}).");
        if (payment.PaymentType == Full && payment.Amount != invoice.TotalAmount)
            errors.Add(nameof(payment.Amount), $"A Full payment must equal the invoice total of {invoice.TotalAmount:N2}.");
        if (payment.Amount > remaining)
            errors.Add(nameof(payment.Amount),
                $"Amount exceeds the remaining balance of {remaining:N2} for order {payment.OrderId}.");
        errors.ThrowIfAny();
    }

    // Rule 7: Downpayment or Full payment locks the order.
    private static bool LocksOrder(string paymentType)
        => paymentType == Downpayment || paymentType == Full;
}
