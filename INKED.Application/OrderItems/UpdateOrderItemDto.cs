using System.ComponentModel.DataAnnotations;

namespace INKED.Application.OrderItems;

/// <summary>
/// Data the client may change on an order line. OrderId is the parent link and is protected.
/// Changes are refused once the order is locked (downpayment paid).
/// </summary>
public class UpdateOrderItemDto
{
    /// <summary>Must be a product that appears in the order's quotation.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be an existing product ID.")]
    public int ProductId { get; set; }

    /// <summary>Must be greater than 0.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }

    /// <summary>DECIMAL(10,2), 0 or more.</summary>
    [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "UnitPrice must be from 0 to 99,999,999.99.")]
    public decimal UnitPrice { get; set; }

    /// <summary>VARCHAR(500), optional.</summary>
    [StringLength(500)]
    public string? ProductionNotes { get; set; }
}
