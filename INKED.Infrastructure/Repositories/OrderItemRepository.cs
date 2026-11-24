using Microsoft.EntityFrameworkCore;
using INKED.Application.OrderItems;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;

namespace INKED.Infrastructure.Repositories;

/// <summary>EF Core data access for dbo.OrderItems. Uses the shared INKEDDbContext.</summary>
public class OrderItemRepository : IOrderItemRepository
{
    private readonly INKEDDbContext _context;

    public OrderItemRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public Task<List<OrderItemResponseDto>> GetAllAsync()
        => _context.OrderItems.AsNoTracking()
            .OrderBy(x => x.OrderId).ThenBy(x => x.OrderItemId)
            .Select(x => new OrderItemResponseDto
            {
                OrderItemId = x.OrderItemId,
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                ProductionNotes = x.ProductionNotes,
                LineTotal = x.LineTotal ?? x.Quantity * x.UnitPrice
            })
            .ToListAsync();

    public Task<OrderItemResponseDto?> GetByIdAsync(int id)
        => _context.OrderItems.AsNoTracking()
            .Where(x => x.OrderItemId == id)
            .Select(x => new OrderItemResponseDto
            {
                OrderItemId = x.OrderItemId,
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                ProductionNotes = x.ProductionNotes,
                LineTotal = x.LineTotal ?? x.Quantity * x.UnitPrice
            })
            .FirstOrDefaultAsync();

    public async Task<OrderItemResponseDto> AddAsync(CreateOrderItemDto dto)
    {
        var entity = new OrderItem
        {
            OrderId = dto.OrderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity ?? 0,      // always filled in by the service
            UnitPrice = dto.UnitPrice ?? 0m,   // always filled in by the service
            ProductionNotes = dto.ProductionNotes
            // LineTotal is computed by SQL Server and read back after saving
        };

        _context.OrderItems.Add(entity);
        await SaveAsync();
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrderItemDto dto)
    {
        var entity = await _context.OrderItems.FirstOrDefaultAsync(x => x.OrderItemId == id);
        if (entity is null) return false;

        entity.ProductId = dto.ProductId;
        entity.Quantity = dto.Quantity;
        entity.UnitPrice = dto.UnitPrice;
        entity.ProductionNotes = dto.ProductionNotes;
        await SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.OrderItems.FirstOrDefaultAsync(x => x.OrderItemId == id);
        if (entity is null) return false;

        _context.OrderItems.Remove(entity);
        await SaveAsync();
        return true;
    }

    public Task<int?> GetOrderQuotationIdAsync(int orderId)
        => _context.Orders.AsNoTracking()
            .Where(o => o.OrderId == orderId)
            .Select(o => (int?)o.QuotationId)
            .FirstOrDefaultAsync();

    public Task<bool> IsOrderLockedAsync(int orderId)
        => _context.Orders.AnyAsync(o => o.OrderId == orderId && o.IsLocked);

    public Task<bool> ProductExistsAsync(int productId)
        => _context.Products.AnyAsync(p => p.ProductId == productId);

    public Task<List<QuotedLineDto>> GetQuotedLinesAsync(int quotationId, int productId)
        => _context.QuotationItems.AsNoTracking()
            .Where(q => q.QuotationId == quotationId && q.ProductId == productId)
            .Select(q => new QuotedLineDto { Quantity = q.Quantity, UnitPrice = q.UnitPrice })
            .ToListAsync();

    private static OrderItemResponseDto ToResponse(OrderItem x)
        => new()
        {
            OrderItemId = x.OrderItemId,
            OrderId = x.OrderId,
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            ProductionNotes = x.ProductionNotes,
            LineTotal = x.LineTotal ?? x.Quantity * x.UnitPrice
        };

    // Every save goes through here so a database rule violation becomes a clean 409 instead of a 500.
    private async Task SaveAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                "The database rejected this change (duplicate value, invalid link, or CHECK constraint).", ex);
        }
    }
}
