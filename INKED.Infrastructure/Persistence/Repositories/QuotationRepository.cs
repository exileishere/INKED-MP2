using INKED.Application.Repositories;
using INKED.Domain.Entities.dbo;
using INKED.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Persistence.Repositories;

public class QuotationRepository : IQuotationRepository
{
    private readonly DatabaseContext _context;

    public QuotationRepository(DatabaseContext context) => _context = context;

    public async Task<IReadOnlyList<Quotation>> GetAllAsync(CancellationToken ct = default)
        => await _context.Quotations.AsNoTracking().OrderBy(e => e.QuotationId).ToListAsync(ct);

    public Task<Quotation?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Quotations.FirstOrDefaultAsync(e => e.QuotationId == id, ct);

    public async Task<Quotation> AddAsync(Quotation entity, CancellationToken ct = default)
    {
        _context.Quotations.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(Quotation entity, CancellationToken ct = default)
    {
        // The entity is already tracked (loaded by GetByIdAsync), so only changed columns are written.
        // Update() is avoided on purpose: it would also mark computed/default columns as modified.
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Quotations.FirstOrDefaultAsync(e => e.QuotationId == id, ct);
        if (entity is null) return false;

        _context.Quotations.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> InquiryExistsAsync(int inquiryId, CancellationToken ct = default)
        => _context.Inquiries.AnyAsync(i => i.InquiryId == inquiryId, ct);

    public Task<bool> UserExistsAsync(int userId, CancellationToken ct = default)
        => _context.UserAccounts.AnyAsync(u => u.UserId == userId, ct);

    public Task<bool> HasItemsAsync(int quotationId, CancellationToken ct = default)
        => _context.QuotationItems.AnyAsync(i => i.QuotationId == quotationId, ct);

    public Task<bool> HasOrderAsync(int quotationId, CancellationToken ct = default)
        => _context.Orders.AnyAsync(o => o.QuotationId == quotationId, ct);
}
