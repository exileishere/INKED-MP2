using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.QuotationItems;

/// <summary>QuotationId is fixed once an item is created. LineTotal is computed by SQL Server and can never be sent.</summary>
public class UpdateQuotationItemDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal UnitPrice { get; set; }

    [StringLength(300)]
    public string? DesignNotes { get; set; }
}
