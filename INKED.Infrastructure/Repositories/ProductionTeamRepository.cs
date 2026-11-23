using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class ProductionTeamRepository : IProductionTeamRepository
{
    private readonly INKEDDbContext _context;

    public ProductionTeamRepository(INKEDDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProductionTeamResponseDto>> GetAllAsync()
    {
        var items = await _context.ProductionTeams.AsNoTracking().OrderBy(t => t.TeamId).ToListAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<ProductionTeamResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.ProductionTeams.AsNoTracking().FirstOrDefaultAsync(t => t.TeamId == id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ProductionTeamResponseDto> AddAsync(ProductionTeamResponseDto team)
    {
        var entity = new ProductionTeam { TeamName = team.TeamName };
        _context.ProductionTeams.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task UpdateAsync(ProductionTeamResponseDto team)
    {
        var entity = await _context.ProductionTeams.FirstAsync(t => t.TeamId == team.TeamID);
        entity.TeamName = team.TeamName;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductionTeams.FirstOrDefaultAsync(t => t.TeamId == id);
        if (entity is null) return;
        _context.ProductionTeams.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.ProductionTeams.AnyAsync(t => t.TeamId == id);

    public async Task<bool> NameExistsAsync(string teamName, int? excludeId = null)
        => await _context.ProductionTeams.AnyAsync(t =>
            t.TeamName == teamName && (excludeId == null || t.TeamId != excludeId));

    public async Task<bool> HasDependentsAsync(int id)
        => await _context.Staff.AnyAsync(s => s.TeamId == id)
        || await _context.FeasibilityChecks.AnyAsync(f => f.TeamId == id);

    private static ProductionTeamResponseDto ToDto(ProductionTeam t) => new()
    {
        TeamID = t.TeamId,
        TeamName = t.TeamName
    };
}
