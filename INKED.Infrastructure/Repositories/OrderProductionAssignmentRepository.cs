using INKED.Application.DTOs;
using INKED.Application.Interfaces;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Repositories;

public class OrderProductionAssignmentRepository : IOrderProductionAssignmentRepository
{
    private readonly INKEDDbContext _context;

    public OrderProductionAssignmentRepository(INKEDDbContext context) => _context = context;

    public async Task<IReadOnlyList<OrderProductionAssignmentResponseDto>> GetAllAsync()
    {
        var items = await _context.OrderProductionAssignments.AsNoTracking().OrderBy(a => a.AssignmentId).ToListAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<OrderProductionAssignmentResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.OrderProductionAssignments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssignmentId == id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<OrderProductionAssignmentResponseDto> AddAsync(OrderProductionAssignmentResponseDto assignment)
    {
        var entity = new OrderProductionAssignment
        {
            OrderId = assignment.OrderID,
            StaffId = assignment.StaffID,
            DateAssigned = assignment.DateAssigned,
            ProductionStatus = assignment.ProductionStatus,
            StatusUpdatedAt = assignment.StatusUpdatedAt,
            DueDate = assignment.DueDate
        };
        _context.OrderProductionAssignments.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task UpdateAsync(OrderProductionAssignmentResponseDto assignment)
    {
        var entity = await _context.OrderProductionAssignments.FirstAsync(a => a.AssignmentId == assignment.AssignmentID);
        entity.ProductionStatus = assignment.ProductionStatus;
        entity.StatusUpdatedAt = assignment.StatusUpdatedAt;
        entity.DueDate = assignment.DueDate;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.OrderProductionAssignments.FirstOrDefaultAsync(a => a.AssignmentId == id);
        if (entity is null) return;
        _context.OrderProductionAssignments.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> OrderExistsAsync(int orderId)
        => await _context.Orders.AnyAsync(o => o.OrderId == orderId);

    public async Task<bool> PairExistsAsync(int orderId, int staffId)
        => await _context.OrderProductionAssignments.AnyAsync(a => a.OrderId == orderId && a.StaffId == staffId);

    private static OrderProductionAssignmentResponseDto ToDto(OrderProductionAssignment a) => new()
    {
        AssignmentID = a.AssignmentId,
        OrderID = a.OrderId,
        StaffID = a.StaffId,
        DateAssigned = a.DateAssigned,
        ProductionStatus = a.ProductionStatus,
        StatusUpdatedAt = a.StatusUpdatedAt,
        DueDate = a.DueDate
    };
}
