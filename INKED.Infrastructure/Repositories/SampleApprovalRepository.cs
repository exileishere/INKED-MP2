using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class SampleApprovalRepository : ISampleApprovalRepository
{
    private readonly INKEDDbContext _context;

    public SampleApprovalRepository(INKEDDbContext context) => _context = context;

    public async Task<IReadOnlyList<SampleApprovalResponseDto>> GetAllAsync()
    {
        var items = await _context.SampleApprovals.AsNoTracking().OrderBy(s => s.SampleApprovalId).ToListAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<SampleApprovalResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.SampleApprovals.AsNoTracking()
            .FirstOrDefaultAsync(s => s.SampleApprovalId == id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<SampleApprovalResponseDto> AddAsync(SampleApprovalResponseDto sample)
    {
        var entity = new SampleApproval
        {
            OrderId = sample.OrderID,
            RevisionNo = sample.RevisionNo,
            SamplePhotoPath = sample.SamplePhotoPath,
            DateSent = sample.DateSent,
            ApprovalStatus = sample.ApprovalStatus,
            DateResponded = sample.DateResponded,
            CustomerRemarks = sample.CustomerRemarks
        };
        _context.SampleApprovals.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task UpdateAsync(SampleApprovalResponseDto sample)
    {
        var entity = await _context.SampleApprovals.FirstAsync(s => s.SampleApprovalId == sample.SampleApprovalID);
        entity.SamplePhotoPath = sample.SamplePhotoPath;
        entity.ApprovalStatus = sample.ApprovalStatus;
        entity.DateResponded = sample.DateResponded;
        entity.CustomerRemarks = sample.CustomerRemarks;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.SampleApprovals.FirstOrDefaultAsync(s => s.SampleApprovalId == id);
        if (entity is null) return;
        _context.SampleApprovals.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> OrderExistsAsync(int orderId)
        => await _context.Orders.AnyAsync(o => o.OrderId == orderId);

    public async Task<bool> RevisionExistsAsync(int orderId, byte revisionNo)
        => await _context.SampleApprovals.AnyAsync(s => s.OrderId == orderId && s.RevisionNo == revisionNo);

    public async Task<int> GetNextRevisionNoAsync(int orderId)
    {
        var max = await _context.SampleApprovals
            .Where(s => s.OrderId == orderId)
            .MaxAsync(s => (byte?)s.RevisionNo);
        return (max ?? 0) + 1;
    }

    private static SampleApprovalResponseDto ToDto(SampleApproval s) => new()
    {
        SampleApprovalID = s.SampleApprovalId,
        OrderID = s.OrderId,
        RevisionNo = s.RevisionNo,
        SamplePhotoPath = s.SamplePhotoPath,
        DateSent = s.DateSent,
        ApprovalStatus = s.ApprovalStatus,
        DateResponded = s.DateResponded,
        CustomerRemarks = s.CustomerRemarks
    };
}
