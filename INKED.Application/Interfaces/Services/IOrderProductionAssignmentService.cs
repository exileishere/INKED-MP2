using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface IOrderProductionAssignmentService
{
    Task<IReadOnlyList<OrderProductionAssignmentResponseDto>> GetAllAsync();
    Task<OrderProductionAssignmentResponseDto> GetByIdAsync(int id);
    Task<OrderProductionAssignmentResponseDto> CreateAsync(CreateOrderProductionAssignmentDto dto);
    Task<OrderProductionAssignmentResponseDto> UpdateAsync(int id, UpdateOrderProductionAssignmentDto dto);
    Task DeleteAsync(int id);
}
