using INKED.Application.Repositories;
using INKED.Domain.Entities.dbo;
using INKED.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Persistence.Repositories;

public class FeasibilityCheckRepository : IFeasibilityCheckRepository
{
    private readonly DatabaseContext _context;

    public FeasibilityCheckRepository(DatabaseContext context) => _context = context;

    public async Task<IReadOnlyList<FeasibilityCheck>> GetAllAsync(CancellationToken ct = default)
        => await _context.FeasibilityChecks.AsNoTracking().OrderBy(e => e.FeasibilityId).ToListAsync(ct);

    public Task<FeasibilityCheck?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.FeasibilityChecks.FirstOrDefaultAsync(e => e.FeasibilityId == id, ct);

    public async Task<FeasibilityCheck> AddAsync(FeasibilityCheck entity, CancellationToken ct = default)
    {
        _context.FeasibilityChecks.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(FeasibilityCheck entity, CancellationToken ct = default)
    {
        // The entity is already tracked (loaded by GetByIdAsync), so only changed columns are written.
        // Update() is avoided on purpose: it would also mark computed/default columns as modified.
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.FeasibilityChecks.FirstOrDefaultAsync(e => e.FeasibilityId == id, ct);
        if (entity is null) return false;

        _context.FeasibilityChecks.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> InquiryExistsAsync(int inquiryId, CancellationToken ct = default)
        => _context.Inquiries.AnyAsync(i => i.InquiryId == inquiryId, ct);

    public Task<bool> TeamExistsAsync(int teamId, CancellationToken ct = default)
        => _context.ProductionTeams.AnyAsync(t => t.TeamId == teamId, ct);
}
