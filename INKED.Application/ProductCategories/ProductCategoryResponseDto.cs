namespace INKED.Application.ProductCategories;

/// <summary>Data returned by the API for a product category.</summary>
public class ProductCategoryResponseDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductionType { get; set; } = string.Empty;
}
