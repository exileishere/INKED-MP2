using Microsoft.EntityFrameworkCore;
using INKED.Application.ProductCategories;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;

namespace INKED.Infrastructure.Repositories;

/// <summary>EF Core data access for dbo.ProductCategories. Uses the shared INKEDDbContext.</summary>
public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly INKEDDbContext _context;

    public ProductCategoryRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public Task<List<ProductCategoryResponseDto>> GetAllAsync()
        => _context.ProductCategories.AsNoTracking()
            .OrderBy(x => x.CategoryId)
            .Select(x => new ProductCategoryResponseDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                ProductionType = x.ProductionType
            })
            .ToListAsync();

    public Task<ProductCategoryResponseDto?> GetByIdAsync(int id)
        => _context.ProductCategories.AsNoTracking()
            .Where(x => x.CategoryId == id)
            .Select(x => new ProductCategoryResponseDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                ProductionType = x.ProductionType
            })
            .FirstOrDefaultAsync();

    public async Task<ProductCategoryResponseDto> AddAsync(CreateProductCategoryDto dto)
    {
        var entity = new ProductCategory
        {
            CategoryName = dto.CategoryName,
            ProductionType = dto.ProductionType
        };

        _context.ProductCategories.Add(entity);
        await SaveAsync();
        return ToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductCategoryDto dto)
    {
        var entity = await _context.ProductCategories.FirstOrDefaultAsync(x => x.CategoryId == id);
        if (entity is null) return false;

        entity.CategoryName = dto.CategoryName;
        entity.ProductionType = dto.ProductionType;
        await SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.ProductCategories.FirstOrDefaultAsync(x => x.CategoryId == id);
        if (entity is null) return false;

        _context.ProductCategories.Remove(entity);
        await SaveAsync();
        return true;
    }

    public Task<bool> CategoryNameExistsAsync(string categoryName, int? excludeCategoryId = null)
        => _context.ProductCategories.AnyAsync(x =>
            x.CategoryName == categoryName &&
            (excludeCategoryId == null || x.CategoryId != excludeCategoryId));

    public Task<bool> HasProductsAsync(int categoryId)
        => _context.Products.AnyAsync(p => p.CategoryId == categoryId);

    private static ProductCategoryResponseDto ToResponse(ProductCategory x)
        => new()
        {
            CategoryId = x.CategoryId,
            CategoryName = x.CategoryName,
            ProductionType = x.ProductionType
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
