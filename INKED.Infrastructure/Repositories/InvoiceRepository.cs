// INKED MP2 | Table: Invoices | Layer: Infrastructure / Repository Implementation (EF Core)
// Owner: Rashel Maxine Baquirin (Billing module)
using INKED.Application.Invoices;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly INKEDDbContext _context;

    public InvoiceRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceResponseDto>> GetAllAsync()
    {
        var invoices = await _context.Invoices.AsNoTracking().OrderBy(i => i.InvoiceId).ToListAsync();
        return invoices.Select(ToDto).ToList();
    }

    public async Task<InvoiceResponseDto?> GetByIdAsync(int id)
    {
        var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.InvoiceId == id);
        return invoice is null ? null : ToDto(invoice);
    }

    // After the save, EF Core reads back the computed DownpaymentAmount column from SQL Server.
    public async Task<InvoiceResponseDto> CreateAsync(InvoiceResponseDto dto)
    {
        var invoice = new Invoice
        {
            OrderId = dto.OrderId,
            InvoiceNo = dto.InvoiceNo,
            InvoiceDate = dto.InvoiceDate,
            TotalAmount = dto.TotalAmount,
            DownpaymentRate = dto.DownpaymentRate,
            Status = dto.Status,
            ConfirmedAt = dto.ConfirmedAt
        };
        _context.Invoices.Add(invoice);
        await _context.SaveBillingChangesAsync();
        return ToDto(invoice);
    }

    public async Task<InvoiceResponseDto?> UpdateAsync(InvoiceResponseDto dto)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == dto.InvoiceId);
        if (invoice is null) return null;

        // OrderId never changes: an invoice always belongs to the order it was issued for.
        invoice.InvoiceNo = dto.InvoiceNo;
        invoice.InvoiceDate = dto.InvoiceDate;
        invoice.TotalAmount = dto.TotalAmount;
        invoice.DownpaymentRate = dto.DownpaymentRate;
        invoice.Status = dto.Status;
        invoice.ConfirmedAt = dto.ConfirmedAt;
        await _context.SaveBillingChangesAsync();
        return ToDto(invoice);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == id);
        if (invoice is null) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveBillingChangesAsync();
        return true;
    }

    public Task<bool> OrderExistsAsync(int orderId)
        => _context.Orders.AnyAsync(o => o.OrderId == orderId);

    // UQ_Invoices_OrderID: one order is billed through one invoice.
    public Task<bool> OrderHasInvoiceAsync(int orderId)
        => _context.Invoices.AnyAsync(i => i.OrderId == orderId);

    // UQ_Invoices_InvoiceNo: every printed invoice number is different.
    public Task<bool> InvoiceNoExistsAsync(string invoiceNo, int? excludeInvoiceId = null)
        => _context.Invoices.AnyAsync(i => i.InvoiceNo == invoiceNo
                                           && (excludeInvoiceId == null || i.InvoiceId != excludeInvoiceId));

    public async Task<decimal> GetTotalPaidForOrderAsync(int orderId)
        => await _context.Payments.Where(p => p.OrderId == orderId).SumAsync(p => (decimal?)p.Amount) ?? 0m;

    internal static InvoiceResponseDto ToDto(Invoice i) => new()
    {
        InvoiceId = i.InvoiceId,
        OrderId = i.OrderId,
        InvoiceNo = i.InvoiceNo,
        InvoiceDate = i.InvoiceDate,
        TotalAmount = i.TotalAmount,
        DownpaymentRate = i.DownpaymentRate,
        DownpaymentAmount = i.DownpaymentAmount,
        Status = i.Status,
        ConfirmedAt = i.ConfirmedAt
    };
}
