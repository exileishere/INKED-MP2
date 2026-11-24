using Microsoft.EntityFrameworkCore;
using INKED.Application.SizeCharts;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;

namespace INKED.Infrastructure.Repositories;

/// <summary>EF Core data access for dbo.SizeCharts. Uses the shared INKEDDbContext.</summary>
public class SizeChartRepository : ISizeChartRepository
{
    private readonly INKEDDbContext _context;

    public SizeChartRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public Task<List<SizeChartResponseDto>> GetAllAsync()
        => _context.SizeCharts.AsNoTracking()
            .OrderBy(x => x.QuotationItemId).ThenBy(x => x.SizeChartId)
            .Select(x => new SizeChartResponseDto
            {
                SizeChartId = x.SizeChartId,
                QuotationItemId = x.QuotationItemId,
                Size = x.Size,
                Quantity = x.Quantity
            })
            .ToListAsync();

    public Task<SizeChartResponseDto?> GetByIdAsync(int id)
        => _context.SizeCharts.AsNoTracking()
            .Where(x => x.SizeChartId == id)
            .Select(x => new SizeChartResponseDto
            {
                SizeChartId = x.SizeChartId,
                QuotationItemId = x.QuotationItemId,
                Size = x.Size,
                Quantity = x.Quantity
            })
            .FirstOrDefaultAsync();

    public async Task<SizeChartResponseDto> AddAsync(CreateSizeChartDto dto)
    {
        var entity = new SizeChart
        {
            QuotationItemId = dto.QuotationItemId,
            Size = dto.Size,
            Quantity = dto.Quantity
        };

        _context.SizeCharts.Add(entity);
        await SaveAsync();
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateSizeChartDto dto)
    {
        var entity = await _context.SizeCharts.FirstOrDefaultAsync(x => x.SizeChartId == id);
        if (entity is null) return false;

        entity.Size = dto.Size;
        entity.Quantity = dto.Quantity;
        await SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.SizeCharts.FirstOrDefaultAsync(x => x.SizeChartId == id);
        if (entity is null) return false;

        _context.SizeCharts.Remove(entity);
        await SaveAsync();
        return true;
    }

    public Task<int?> GetQuotedQuantityAsync(int quotationItemId)
        => _context.QuotationItems.AsNoTracking()
            .Where(q => q.QuotationItemId == quotationItemId)
            .Select(q => (int?)q.Quantity)
            .FirstOrDefaultAsync();

    public Task<bool> SizeExistsForQuotationItemAsync(int quotationItemId, string size, int? excludeSizeChartId = null)
        => _context.SizeCharts.AnyAsync(x =>
            x.QuotationItemId == quotationItemId &&
            x.Size == size &&
            (excludeSizeChartId == null || x.SizeChartId != excludeSizeChartId));

    public Task<int> GetTotalSizeQuantityAsync(int quotationItemId, int? excludeSizeChartId = null)
        => _context.SizeCharts
            .Where(x => x.QuotationItemId == quotationItemId &&
                        (excludeSizeChartId == null || x.SizeChartId != excludeSizeChartId))
            .SumAsync(x => x.Quantity);

    // Locked = the quotation of this item already became an order with IsLocked = 1
    public Task<bool> IsQuotationItemLockedAsync(int quotationItemId)
        => (from q in _context.QuotationItems
            join o in _context.Orders on q.QuotationId equals o.QuotationId
            where q.QuotationItemId == quotationItemId && o.IsLocked
            select o.OrderId).AnyAsync();

    private static SizeChartResponseDto ToResponse(SizeChart x)
        => new()
        {
            SizeChartId = x.SizeChartId,
            QuotationItemId = x.QuotationItemId,
            Size = x.Size,
            Quantity = x.Quantity
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
