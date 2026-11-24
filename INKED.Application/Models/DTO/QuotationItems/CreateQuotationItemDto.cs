using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.QuotationItems;

public class CreateQuotationItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "QuotationId must be a valid Quotations.QuotationID.")]
    public int QuotationId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a valid Products.ProductID.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }

    /// <summary>DECIMAL(10,2), cannot be negative.</summary>
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal UnitPrice { get; set; }

    [StringLength(300)]
    public string? DesignNotes { get; set; }
}
