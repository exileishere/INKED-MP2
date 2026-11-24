using INKED.Application.Repositories;
using INKED.Domain.Entities.dbo;
using INKED.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Persistence.Repositories;

public class QuotationItemRepository : IQuotationItemRepository
{
    private readonly DatabaseContext _context;

    public QuotationItemRepository(DatabaseContext context) => _context = context;

    public async Task<IReadOnlyList<QuotationItem>> GetAllAsync(CancellationToken ct = default)
        => await _context.QuotationItems.AsNoTracking().OrderBy(e => e.QuotationItemId).ToListAsync(ct);

    public Task<QuotationItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.QuotationItems.FirstOrDefaultAsync(e => e.QuotationItemId == id, ct);

    public async Task<QuotationItem> AddAsync(QuotationItem entity, CancellationToken ct = default)
    {
        _context.QuotationItems.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(QuotationItem entity, CancellationToken ct = default)
    {
        // The entity is already tracked (loaded by GetByIdAsync), so only changed columns are written.
        // Update() is avoided on purpose: it would also mark computed/default columns as modified.
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.QuotationItems.FirstOrDefaultAsync(e => e.QuotationItemId == id, ct);
        if (entity is null) return false;

        _context.QuotationItems.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> QuotationExistsAsync(int quotationId, CancellationToken ct = default)
        => _context.Quotations.AnyAsync(q => q.QuotationId == quotationId, ct);

    public Task<bool> ProductExistsAsync(int productId, CancellationToken ct = default)
        => _context.Products.AnyAsync(p => p.ProductId == productId, ct);

    public Task<bool> HasSizeChartsAsync(int quotationItemId, CancellationToken ct = default)
        => _context.SizeCharts.AnyAsync(s => s.QuotationItemId == quotationItemId, ct);
}
