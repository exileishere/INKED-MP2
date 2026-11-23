namespace INKED.Application.OrderItems;

/// <summary>Application operations for dbo.OrderItems.</summary>
public interface IOrderItemService
{
    Task<List<OrderItemResponseDto>> GetAllAsync();
    Task<OrderItemResponseDto?> GetByIdAsync(int id);
    Task<OrderItemResponseDto> CreateAsync(CreateOrderItemDto dto);
    Task<bool> UpdateAsync(int id, UpdateOrderItemDto dto);
    Task<bool> DeleteAsync(int id);
}
