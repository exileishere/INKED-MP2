namespace INKED.Application.ProductCategories;

/// <summary>
/// Business rules for dbo.ProductCategories, taken from the MP1 script:
///   UQ_ProductCategories_CategoryName, CK_ProductCategories_CategoryName (not blank),
///   CK_ProductCategories_ProductionType (4 allowed values), FK_Products_ProductCategories.

/// Errors used by this service (the controller turns them into HTTP codes):
///   ArgumentException         -> 400 Bad Request  (invalid value or missing linked record)
///   InvalidOperationException -> 409 Conflict     (duplicate, record in use, or order locked)
/// </summary>
public class ProductCategoryService : IProductCategoryService
{
    // Same values as CK_ProductCategories_ProductionType in the MP1 script
    private static readonly string[] AllowedProductionTypes =
        { "Apparel/Print", "Tailoring", "Embroidery/Enamel", "Paper" };

    private readonly IProductCategoryRepository _repository;

    public ProductCategoryService(IProductCategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<ProductCategoryResponseDto>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<ProductCategoryResponseDto?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<ProductCategoryResponseDto> CreateAsync(CreateProductCategoryDto dto)
    {
        var clean = new CreateProductCategoryDto
        {
            CategoryName = CleanName(dto.CategoryName),
            ProductionType = CleanProductionType(dto.ProductionType)
        };

        if (await _repository.CategoryNameExistsAsync(clean.CategoryName))
            throw new InvalidOperationException($"Category name '{clean.CategoryName}' already exists.");

        return await _repository.AddAsync(clean);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductCategoryDto dto)
    {
        if (await _repository.GetByIdAsync(id) is null) return false;

        var clean = new UpdateProductCategoryDto
        {
            CategoryName = CleanName(dto.CategoryName),
            ProductionType = CleanProductionType(dto.ProductionType)
        };

        if (await _repository.CategoryNameExistsAsync(clean.CategoryName, id))
            throw new InvalidOperationException($"Category name '{clean.CategoryName}' already exists.");

        return await _repository.UpdateAsync(id, clean);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _repository.GetByIdAsync(id) is null) return false;

        // FK_Products_ProductCategories: a category that still has products cannot be removed
        if (await _repository.HasProductsAsync(id))
            throw new InvalidOperationException(
                $"Category {id} cannot be deleted because products still use it.");

        return await _repository.DeleteAsync(id);
    }

    private static string CleanName(string? value)
    {
        var name = (value ?? string.Empty).Trim();
        if (name.Length == 0)
            throw new ArgumentException("CategoryName is required and cannot be blank.");
        if (name.Length > 50)
            throw new ArgumentException("CategoryName must be 50 characters or fewer.");
        return name;
    }

    private static string CleanProductionType(string? value)
    {
        var type = (value ?? string.Empty).Trim();
        var match = AllowedProductionTypes.FirstOrDefault(t =>
            string.Equals(t, type, StringComparison.OrdinalIgnoreCase));
        if (match is null)
            throw new ArgumentException(
                $"ProductionType must be one of: {string.Join(", ", AllowedProductionTypes)}.");
        return match;   // saved with the exact spelling the CHECK constraint expects
    }
}
