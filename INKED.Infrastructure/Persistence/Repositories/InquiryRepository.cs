using INKED.Application.Repositories;
using INKED.Domain.Entities.dbo;
using INKED.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Persistence.Repositories;

public class InquiryRepository : IInquiryRepository
{
    private readonly DatabaseContext _context;

    public InquiryRepository(DatabaseContext context) => _context = context;

    public async Task<IReadOnlyList<Inquiry>> GetAllAsync(CancellationToken ct = default)
        => await _context.Inquiries.AsNoTracking().OrderBy(e => e.InquiryId).ToListAsync(ct);

    public Task<Inquiry?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Inquiries.FirstOrDefaultAsync(e => e.InquiryId == id, ct);

    public async Task<Inquiry> AddAsync(Inquiry entity, CancellationToken ct = default)
    {
        _context.Inquiries.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(Inquiry entity, CancellationToken ct = default)
    {
        // The entity is already tracked (loaded by GetByIdAsync), so only changed columns are written.
        // Update() is avoided on purpose: it would also mark computed/default columns as modified.
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Inquiries.FirstOrDefaultAsync(e => e.InquiryId == id, ct);
        if (entity is null) return false;

        _context.Inquiries.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> CustomerExistsAsync(int customerId, CancellationToken ct = default)
        => _context.Customers.AnyAsync(c => c.CustomerId == customerId, ct);

    public Task<bool> UserExistsAsync(int userId, CancellationToken ct = default)
        => _context.UserAccounts.AnyAsync(u => u.UserId == userId, ct);

    public Task<bool> HasQuotationsAsync(int inquiryId, CancellationToken ct = default)
        => _context.Quotations.AnyAsync(q => q.InquiryId == inquiryId, ct);

    public Task<bool> HasFeasibilityChecksAsync(int inquiryId, CancellationToken ct = default)
        => _context.FeasibilityChecks.AnyAsync(f => f.InquiryId == inquiryId, ct);
}
