// INKED MP2 | Table: Invoices | Layer: Application / Service Implementation
// Owner: Rashel Maxine Baquirin (Billing module)
//
// Business rules (from the MP1 database design, Table 11):
//  1. The order must exist, and each order has only one invoice (UQ_Invoices_OrderID).
//  2. InvoiceNo is required, not blank, max 20 characters and unique (UQ_Invoices_InvoiceNo, CK_Invoices_InvoiceNo).
//  3. TotalAmount must be greater than 0 (CK_Invoices_TotalAmount).
//  4. DownpaymentRate defaults to 0.60 and must be > 0 and <= 1 (CK_Invoices_DownpaymentRate).
//  5. DownpaymentAmount is computed by SQL Server and is never typed in.
//  6. Status defaults to 'Sent' and must be Draft, Sent, Confirmed or Cancelled (CK_Invoices_Status).
//  7. ConfirmedAt is required when Status is Confirmed (CK_Invoices_ConfirmedAt); the API fills in the
//     current time if it is missing, and it cannot be earlier than the invoice date.
//  8. On update, TotalAmount cannot go below the amount already paid for the order.
//  9. A Confirmed invoice is a final billing document and cannot be deleted (cancel it instead).
using INKED.Application.Billing;

namespace INKED.Application.Invoices;

public class InvoiceService : IInvoiceService
{
    private const decimal DefaultDownpaymentRate = 0.60m;
    private const string DefaultStatus = "Sent";

    private readonly IInvoiceRepository _repository;

    public InvoiceService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public Task<List<InvoiceResponseDto>> GetAllAsync() => _repository.GetAllAsync();

    public Task<InvoiceResponseDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto)
    {
        var invoice = new InvoiceResponseDto
        {
            OrderId = dto.OrderId!.Value,
            InvoiceNo = dto.InvoiceNo.Trim(),
            InvoiceDate = dto.InvoiceDate ?? DateOnly.FromDateTime(DateTime.Today),
            TotalAmount = dto.TotalAmount!.Value,
            DownpaymentRate = dto.DownpaymentRate ?? DefaultDownpaymentRate,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? DefaultStatus : dto.Status.Trim(),
            ConfirmedAt = dto.ConfirmedAt
        };
        ApplyConfirmedAt(invoice);

        var errors = new ValidationErrors();
        if (!await _repository.OrderExistsAsync(invoice.OrderId))
            errors.Add(nameof(invoice.OrderId), $"Order with ID {invoice.OrderId} does not exist.");
        ValidateRules(invoice, errors);
        errors.ThrowIfAny();

        if (await _repository.OrderHasInvoiceAsync(invoice.OrderId))
            throw new ConflictException($"Order {invoice.OrderId} already has an invoice. Each order is billed through one invoice.");
        if (await _repository.InvoiceNoExistsAsync(invoice.InvoiceNo))
            throw new ConflictException($"Invoice number '{invoice.InvoiceNo}' is already used.");

        return await _repository.CreateAsync(invoice);
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvoiceDto dto)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice is null) return false;

        invoice.InvoiceNo = dto.InvoiceNo.Trim();
        invoice.InvoiceDate = dto.InvoiceDate!.Value;
        invoice.TotalAmount = dto.TotalAmount!.Value;
        invoice.DownpaymentRate = dto.DownpaymentRate!.Value;
        invoice.Status = dto.Status.Trim();
        invoice.ConfirmedAt = dto.ConfirmedAt;
        ApplyConfirmedAt(invoice);

        var errors = new ValidationErrors();
        ValidateRules(invoice, errors);
        var totalPaid = await _repository.GetTotalPaidForOrderAsync(invoice.OrderId);
        if (invoice.TotalAmount < totalPaid)
            errors.Add(nameof(invoice.TotalAmount),
                $"TotalAmount cannot be lower than the {totalPaid:N2} already paid for order {invoice.OrderId}.");
        errors.ThrowIfAny();

        if (await _repository.InvoiceNoExistsAsync(invoice.InvoiceNo, id))
            throw new ConflictException($"Invoice number '{invoice.InvoiceNo}' is already used by another invoice.");

        return await _repository.UpdateAsync(invoice) is not null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice is null) return false;

        if (invoice.Status == "Confirmed")
            throw new ConflictException(
                $"Invoice {id} was already confirmed by the customer and cannot be deleted. Update its Status to 'Cancelled' instead.");

        return await _repository.DeleteAsync(id);
    }

    // CK_Invoices_ConfirmedAt: a Confirmed invoice must have a confirmation time.
    private static void ApplyConfirmedAt(InvoiceResponseDto invoice)
    {
        if (invoice.Status == "Confirmed" && invoice.ConfirmedAt is null)
            invoice.ConfirmedAt = DateTimeHelper.NowToSecond();
    }

    private static void ValidateRules(InvoiceResponseDto invoice, ValidationErrors errors)
    {
        if (invoice.ConfirmedAt.HasValue &&
            DateOnly.FromDateTime(invoice.ConfirmedAt.Value) < invoice.InvoiceDate)
            errors.Add(nameof(invoice.ConfirmedAt), "ConfirmedAt cannot be earlier than InvoiceDate.");
    }
}
