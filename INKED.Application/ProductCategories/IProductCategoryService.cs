namespace INKED.Application.ProductCategories;

/// <summary>Application operations for dbo.ProductCategories.</summary>
public interface IProductCategoryService
{
    Task<List<ProductCategoryResponseDto>> GetAllAsync();
    Task<ProductCategoryResponseDto?> GetByIdAsync(int id);
    Task<ProductCategoryResponseDto> CreateAsync(CreateProductCategoryDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}
