namespace INKED.Application.OrderItems;

/// <summary>
/// Helper DTO (read-only): the Quantity and UnitPrice of a quotation line, used to copy
/// values from the accepted quotation into a new order item (MP1 Table 10).
/// </summary>
public class QuotedLineDto
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
