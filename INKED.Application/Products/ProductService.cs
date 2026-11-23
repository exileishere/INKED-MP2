namespace INKED.Application.Products;

/// <summary>
/// Business rules for dbo.Products, taken from the MP1 script and documentation (Table 6):
///   FK_Products_ProductCategories, UQ_Products_ProductName, CK_Products_ProductName (not blank),
///   CK_Products_BasePrice (>= 0), DF_Products_IsActive (default 1).
///   "IsActive lets the shop retire a product without deleting it, so old quotations and
///   orders still point to it" -> a product used in QuotationItems/OrderItems cannot be deleted.

/// Errors used by this service (the controller turns them into HTTP codes):
///   ArgumentException         -> 400 Bad Request  (invalid value or missing linked record)
///   InvalidOperationException -> 409 Conflict     (duplicate, record in use, or order locked)
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<ProductResponseDto>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<ProductResponseDto?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var clean = new CreateProductDto
        {
            CategoryId = dto.CategoryId,
            ProductName = CleanName(dto.ProductName),
            Description = CleanDescription(dto.Description),
            BasePrice = ValidPrice(dto.BasePrice),
            IsActive = dto.IsActive
        };

        await EnsureCategoryExists(clean.CategoryId);

        if (await _repository.ProductNameExistsAsync(clean.ProductName))
            throw new InvalidOperationException($"Product name '{clean.ProductName}' already exists.");

        return await _repository.AddAsync(clean);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
    {
        if (await _repository.GetByIdAsync(id) is null) return false;

        var clean = new UpdateProductDto
        {
            CategoryId = dto.CategoryId,
            ProductName = CleanName(dto.ProductName),
            Description = CleanDescription(dto.Description),
            BasePrice = ValidPrice(dto.BasePrice),
            IsActive = dto.IsActive
        };

        await EnsureCategoryExists(clean.CategoryId);

        if (await _repository.ProductNameExistsAsync(clean.ProductName, id))
            throw new InvalidOperationException($"Product name '{clean.ProductName}' already exists.");

        return await _repository.UpdateAsync(id, clean);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _repository.GetByIdAsync(id) is null) return false;

        // FK_QuotationItems_Products and FK_OrderItems_Products
        if (await _repository.IsUsedInQuotationsOrOrdersAsync(id))
            throw new InvalidOperationException(
                $"Product {id} cannot be deleted because quotations or orders use it. Set IsActive to false instead.");

        return await _repository.DeleteAsync(id);
    }

    private async Task EnsureCategoryExists(int categoryId)
    {
        if (!await _repository.CategoryExistsAsync(categoryId))
            throw new ArgumentException($"CategoryId {categoryId} does not exist in ProductCategories.");
    }

    private static string CleanName(string? value)
    {
        var name = (value ?? string.Empty).Trim();
        if (name.Length == 0)
            throw new ArgumentException("ProductName is required and cannot be blank.");
        if (name.Length > 100)
            throw new ArgumentException("ProductName must be 100 characters or fewer.");
        return name;
    }

    private static string? CleanDescription(string? value)
    {
        var text = value?.Trim();
        if (string.IsNullOrEmpty(text)) return null;
        if (text.Length > 300)
            throw new ArgumentException("Description must be 300 characters or fewer.");
        return text;
    }

    private static decimal ValidPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("BasePrice cannot be negative.");
        if (price > 99999999.99m)
            throw new ArgumentException("BasePrice is too large for DECIMAL(10,2).");
        if (decimal.Round(price, 2) != price)
            throw new ArgumentException("BasePrice can only have up to 2 decimal places.");
        return price;
    }
}
