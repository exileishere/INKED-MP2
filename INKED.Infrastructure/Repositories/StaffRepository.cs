using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly INKEDDbContext _context;

    public StaffRepository(INKEDDbContext context) => _context = context;

    public async Task<IReadOnlyList<StaffResponseDto>> GetAllAsync()
    {
        var items = await _context.Staff.AsNoTracking().OrderBy(s => s.StaffId).ToListAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<StaffResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Staff.AsNoTracking().FirstOrDefaultAsync(s => s.StaffId == id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<StaffResponseDto> AddAsync(StaffResponseDto staff)
    {
        var entity = new Staff
        {
            TeamId = staff.TeamID,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            ContactNumber = staff.ContactNumber,
            IsActive = staff.IsActive
        };
        _context.Staff.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task UpdateAsync(StaffResponseDto staff)
    {
        var entity = await _context.Staff.FirstAsync(s => s.StaffId == staff.StaffID);
        entity.TeamId = staff.TeamID;
        entity.FirstName = staff.FirstName;
        entity.LastName = staff.LastName;
        entity.ContactNumber = staff.ContactNumber;
        entity.IsActive = staff.IsActive;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Staff.FirstOrDefaultAsync(s => s.StaffId == id);
        if (entity is null) return;
        _context.Staff.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasDependentsAsync(int id)
        => await _context.OrderProductionAssignments.AnyAsync(a => a.StaffId == id)
        || await _context.UserAccounts.AnyAsync(u => u.StaffId == id);

    private static StaffResponseDto ToDto(Staff s) => new()
    {
        StaffID = s.StaffId,
        TeamID = s.TeamId,
        FirstName = s.FirstName,
        LastName = s.LastName,
        ContactNumber = s.ContactNumber,
        IsActive = s.IsActive
    };
}
