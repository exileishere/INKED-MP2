// INKED MP2 | Table: Orders | Layer: Application / Service Interface
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Orders;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync();
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    Task<bool> UpdateAsync(int id, UpdateOrderDto dto);
    Task<bool> DeleteAsync(int id);
}
