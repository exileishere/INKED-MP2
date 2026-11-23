// INKED MP2 | Table: Orders | Layer: Infrastructure / Repository Implementation (EF Core)
// Owner: Rashel Maxine Baquirin (Billing module)
using INKED.Application.Orders;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly INKEDDbContext _context;

    public OrderRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _context.Orders.AsNoTracking().OrderBy(o => o.OrderId).ToListAsync();
        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == id);
        return order is null ? null : ToDto(order);
    }

    public async Task<OrderResponseDto> CreateAsync(OrderResponseDto dto)
    {
        var order = new Order
        {
            QuotationId = dto.QuotationId,
            OrderDate = dto.OrderDate,
            RequiredDate = dto.RequiredDate,
            IsLocked = dto.IsLocked
        };
        _context.Orders.Add(order);
        await _context.SaveBillingChangesAsync();
        return ToDto(order);
    }

    public async Task<bool> UpdateAsync(OrderResponseDto dto)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);
        if (order is null) return false;

        // QuotationId and OrderDate never change after the order is created.
        order.RequiredDate = dto.RequiredDate;
        order.IsLocked = dto.IsLocked;
        await _context.SaveBillingChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
        if (order is null) return false;

        _context.Orders.Remove(order);
        await _context.SaveBillingChangesAsync();
        return true;
    }

    public Task<string?> GetQuotationStatusAsync(int quotationId)
        => _context.Quotations
            .Where(q => q.QuotationId == quotationId)
            .Select(q => (string?)q.Status)
            .FirstOrDefaultAsync();

    // UQ_Orders_QuotationID: one accepted quotation becomes exactly one order.
    public Task<bool> QuotationHasOrderAsync(int quotationId)
        => _context.Orders.AnyAsync(o => o.QuotationId == quotationId);

    // Orders is the parent of 7 tables. SQL Server blocks deleting an order that is still referenced.
    public async Task<List<string>> GetDependentRecordTypesAsync(int id)
    {
        var found = new List<string>();
        if (await _context.OrderItems.AnyAsync(x => x.OrderId == id)) found.Add("order items");
        if (await _context.Invoices.AnyAsync(x => x.OrderId == id)) found.Add("an invoice");
        if (await _context.Payments.AnyAsync(x => x.OrderId == id)) found.Add("payments");
        if (await _context.SampleApprovals.AnyAsync(x => x.OrderId == id)) found.Add("sample approvals");
        if (await _context.OrderProductionAssignments.AnyAsync(x => x.OrderId == id)) found.Add("production assignments");
        if (await _context.OrderStatusHistories.AnyAsync(x => x.OrderId == id)) found.Add("status history");
        if (await _context.DeliveryRecords.AnyAsync(x => x.OrderId == id)) found.Add("delivery records");
        return found;
    }

    private static OrderResponseDto ToDto(Order o) => new()
    {
        OrderId = o.OrderId,
        QuotationId = o.QuotationId,
        OrderDate = o.OrderDate,
        RequiredDate = o.RequiredDate,
        IsLocked = o.IsLocked
    };
}
