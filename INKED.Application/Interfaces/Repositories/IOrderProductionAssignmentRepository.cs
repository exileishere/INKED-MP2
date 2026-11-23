using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface IOrderProductionAssignmentRepository
{
    Task<IReadOnlyList<OrderProductionAssignmentResponseDto>> GetAllAsync();
    Task<OrderProductionAssignmentResponseDto?> GetByIdAsync(int id);
    Task<OrderProductionAssignmentResponseDto> AddAsync(OrderProductionAssignmentResponseDto assignment); // AssignmentID is ignored
    Task UpdateAsync(OrderProductionAssignmentResponseDto assignment);
    Task DeleteAsync(int id);

    Task<bool> OrderExistsAsync(int orderId);
    /// <summary>UNIQUE (OrderID, StaffID).</summary>
    Task<bool> PairExistsAsync(int orderId, int staffId);
}
