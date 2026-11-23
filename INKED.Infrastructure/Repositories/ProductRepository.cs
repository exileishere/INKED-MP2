using Microsoft.EntityFrameworkCore;
using INKED.Application.Products;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;

namespace INKED.Infrastructure.Repositories;

/// <summary>EF Core data access for dbo.Products. Uses the shared INKEDDbContext.</summary>
public class ProductRepository : IProductRepository
{
    private readonly INKEDDbContext _context;

    public ProductRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public Task<List<ProductResponseDto>> GetAllAsync()
        => _context.Products.AsNoTracking()
            .OrderBy(x => x.ProductId)
            .Select(x => new ProductResponseDto
            {
                ProductId = x.ProductId,
                CategoryId = x.CategoryId,
                ProductName = x.ProductName,
                Description = x.Description,
                BasePrice = x.BasePrice,
                IsActive = x.IsActive
            })
            .ToListAsync();

    public Task<ProductResponseDto?> GetByIdAsync(int id)
        => _context.Products.AsNoTracking()
            .Where(x => x.ProductId == id)
            .Select(x => new ProductResponseDto
            {
                ProductId = x.ProductId,
                CategoryId = x.CategoryId,
                ProductName = x.ProductName,
                Description = x.Description,
                BasePrice = x.BasePrice,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();

    public async Task<ProductResponseDto> AddAsync(CreateProductDto dto)
    {
        var entity = new Product
        {
            CategoryId = dto.CategoryId,
            ProductName = dto.ProductName,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            IsActive = dto.IsActive
        };

        _context.Products.Add(entity);
        await SaveAsync();

        // IsActive has a SQL default of 1 (DF_Products_IsActive). EF Core does not send "false"
        // on insert because false is the C# default, so SQL saves 1. Correct it right away.
        if (!dto.IsActive && entity.IsActive)
        {
            entity.IsActive = false;
            await SaveAsync();
        }

        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity is null) return false;

        entity.CategoryId = dto.CategoryId;
        entity.ProductName = dto.ProductName;
        entity.Description = dto.Description;
        entity.BasePrice = dto.BasePrice;
        entity.IsActive = dto.IsActive;
        await SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity is null) return false;

        _context.Products.Remove(entity);
        await SaveAsync();
        return true;
    }

    public Task<bool> CategoryExistsAsync(int categoryId)
        => _context.ProductCategories.AnyAsync(c => c.CategoryId == categoryId);

    public Task<bool> ProductNameExistsAsync(string productName, int? excludeProductId = null)
        => _context.Products.AnyAsync(x =>
            x.ProductName == productName &&
            (excludeProductId == null || x.ProductId != excludeProductId));

    public async Task<bool> IsUsedInQuotationsOrOrdersAsync(int productId)
        => await _context.QuotationItems.AnyAsync(q => q.ProductId == productId)
        || await _context.OrderItems.AnyAsync(o => o.ProductId == productId);

    private static ProductResponseDto ToResponse(Product x)
        => new()
        {
            ProductId = x.ProductId,
            CategoryId = x.CategoryId,
            ProductName = x.ProductName,
            Description = x.Description,
            BasePrice = x.BasePrice,
            IsActive = x.IsActive
        };

    // Every save goes through here so a database rule violation becomes a clean 409 instead of a 500.
    private async Task SaveAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                "The database rejected this change (duplicate value, invalid link, or CHECK constraint).", ex);
        }
    }
}
