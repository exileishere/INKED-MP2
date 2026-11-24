namespace INKED.Application.OrderItems;

/// <summary>Data returned by the API for an order line. LineTotal = Quantity x UnitPrice (computed by SQL Server).</summary>
public class OrderItemResponseDto
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ProductionNotes { get; set; }
    public decimal LineTotal { get; set; }
}
