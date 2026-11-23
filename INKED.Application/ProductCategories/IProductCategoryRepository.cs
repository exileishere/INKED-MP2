namespace INKED.Application.ProductCategories;

/// <summary>
/// Data-access operations for dbo.ProductCategories. Implemented in INKED.Infrastructure with EF Core.
/// It works with DTOs so the Application layer does not depend on the scaffolded entity classes.
/// </summary>
public interface IProductCategoryRepository
{
    Task<List<ProductCategoryResponseDto>> GetAllAsync();
    Task<ProductCategoryResponseDto?> GetByIdAsync(int id);
    Task<ProductCategoryResponseDto> AddAsync(CreateProductCategoryDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductCategoryDto dto);
    Task<bool> DeleteAsync(int id);

    // Checks used by the service
    Task<bool> CategoryNameExistsAsync(string categoryName, int? excludeCategoryId = null);
    Task<bool> HasProductsAsync(int categoryId);
}
