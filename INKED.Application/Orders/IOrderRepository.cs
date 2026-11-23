// INKED MP2 | Table: Orders | Layer: Application / Repository Interface
// Owner: Rashel Maxine Baquirin (Billing module)
// Works with DTOs because the scaffolded entities live in INKED.Infrastructure.Models,
// which the Application project cannot reference. The repository maps DTO <-> entity.
namespace INKED.Application.Orders;

public interface IOrderRepository
{
    Task<List<OrderResponseDto>> GetAllAsync();
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<OrderResponseDto> CreateAsync(OrderResponseDto order);
    Task<bool> UpdateAsync(OrderResponseDto order);
    Task<bool> DeleteAsync(int id);

    // Foreign key and business-rule checks
    Task<string?> GetQuotationStatusAsync(int quotationId);
    Task<bool> QuotationHasOrderAsync(int quotationId);
    Task<List<string>> GetDependentRecordTypesAsync(int id);
}
