namespace INKED.Application.Products;

/// <summary>Data returned by the API for a product.</summary>
public class ProductResponseDto
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
}
