namespace INKED.Application.OrderItems;

/// <summary>
/// Data-access operations for dbo.OrderItems. Implemented in INKED.Infrastructure with EF Core.
/// It works with DTOs so the Application layer does not depend on the scaffolded entity classes.
/// </summary>
public interface IOrderItemRepository
{
    Task<List<OrderItemResponseDto>> GetAllAsync();
    Task<OrderItemResponseDto?> GetByIdAsync(int id);
    Task<OrderItemResponseDto> AddAsync(CreateOrderItemDto dto);
    Task<bool> UpdateAsync(int id, UpdateOrderItemDto dto);
    Task<bool> DeleteAsync(int id);

    // Checks used by the service
    /// <summary>Returns the order's QuotationID, or null if the order does not exist.</summary>
    Task<int?> GetOrderQuotationIdAsync(int orderId);
    Task<bool> IsOrderLockedAsync(int orderId);
    Task<bool> ProductExistsAsync(int productId);
    Task<List<QuotedLineDto>> GetQuotedLinesAsync(int quotationId, int productId);
}
