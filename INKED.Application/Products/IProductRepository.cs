namespace INKED.Application.Products;

/// <summary>
/// Data-access operations for dbo.Products. Implemented in INKED.Infrastructure with EF Core.
/// It works with DTOs so the Application layer does not depend on the scaffolded entity classes.
/// </summary>
public interface IProductRepository
{
    Task<List<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> AddAsync(CreateProductDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);

    // Checks used by the service
    Task<bool> CategoryExistsAsync(int categoryId);
    Task<bool> ProductNameExistsAsync(string productName, int? excludeProductId = null);
    Task<bool> IsUsedInQuotationsOrOrdersAsync(int productId);
}
