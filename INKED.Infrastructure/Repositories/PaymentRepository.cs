// INKED MP2 | Table: Payments | Layer: Infrastructure / Repository Implementation (EF Core)
// Owner: Rashel Maxine Baquirin (Billing module)
using INKED.Application.Invoices;
using INKED.Application.Payments;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly INKEDDbContext _context;

    public PaymentRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentResponseDto>> GetAllAsync()
    {
        var payments = await _context.Payments.AsNoTracking().OrderBy(p => p.PaymentId).ToListAsync();
        return payments.Select(ToDto).ToList();
    }

    public async Task<PaymentResponseDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.PaymentId == id);
        return payment is null ? null : ToDto(payment);
    }

    public async Task<PaymentResponseDto> CreateAsync(PaymentResponseDto dto, bool lockOrder)
    {
        var payment = new Payment
        {
            OrderId = dto.OrderId,
            PaymentDate = dto.PaymentDate,
            Amount = dto.Amount,
            PaymentType = dto.PaymentType,
            PaymentMethod = dto.PaymentMethod,
            ReferenceNo = dto.ReferenceNo
        };
        _context.Payments.Add(payment);
        await LockOrderIfNeededAsync(payment.OrderId, lockOrder);
        await _context.SaveBillingChangesAsync(); // payment + order lock are saved together
        return ToDto(payment);
    }

    public async Task<bool> UpdateAsync(PaymentResponseDto dto, bool lockOrder)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == dto.PaymentId);
        if (payment is null) return false;

        // OrderId never changes: a payment always belongs to the order it was received for.
        payment.PaymentDate = dto.PaymentDate;
        payment.Amount = dto.Amount;
        payment.PaymentType = dto.PaymentType;
        payment.PaymentMethod = dto.PaymentMethod;
        payment.ReferenceNo = dto.ReferenceNo;
        await LockOrderIfNeededAsync(payment.OrderId, lockOrder);
        await _context.SaveBillingChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);
        if (payment is null) return false;

        _context.Payments.Remove(payment);
        await _context.SaveBillingChangesAsync();
        return true;
    }

    public Task<bool> OrderExistsAsync(int orderId)
        => _context.Orders.AnyAsync(o => o.OrderId == orderId);

    public async Task<InvoiceResponseDto?> GetInvoiceForOrderAsync(int orderId)
    {
        var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.OrderId == orderId);
        return invoice is null ? null : InvoiceRepository.ToDto(invoice);
    }

    // UQ_Payments_OrderID_PaymentType: at most one Downpayment, one Balance and one Full payment per order.
    public Task<bool> PaymentTypeExistsForOrderAsync(int orderId, string paymentType, int? excludePaymentId = null)
        => _context.Payments.AnyAsync(p => p.OrderId == orderId && p.PaymentType == paymentType
                                           && (excludePaymentId == null || p.PaymentId != excludePaymentId));

    public Task<List<string>> GetPaymentTypesForOrderAsync(int orderId, int? excludePaymentId = null)
        => _context.Payments
            .Where(p => p.OrderId == orderId && (excludePaymentId == null || p.PaymentId != excludePaymentId))
            .Select(p => p.PaymentType)
            .ToListAsync();

    public async Task<decimal> GetTotalPaidForOrderAsync(int orderId, int? excludePaymentId = null)
        => await _context.Payments
            .Where(p => p.OrderId == orderId && (excludePaymentId == null || p.PaymentId != excludePaymentId))
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

    // MP1 rule: Orders.IsLocked is set to 1 once the downpayment is paid.
    private async Task LockOrderIfNeededAsync(int orderId, bool lockOrder)
    {
        if (!lockOrder) return;

        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order is not null && !order.IsLocked)
            order.IsLocked = true;
    }

    private static PaymentResponseDto ToDto(Payment p) => new()
    {
        PaymentId = p.PaymentId,
        OrderId = p.OrderId,
        PaymentDate = p.PaymentDate,
        Amount = p.Amount,
        PaymentType = p.PaymentType,
        PaymentMethod = p.PaymentMethod,
        ReferenceNo = p.ReferenceNo
    };
}
