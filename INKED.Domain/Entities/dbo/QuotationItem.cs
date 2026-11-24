namespace INKED.Domain.Entities.dbo;

/// <summary>
/// Domain representation used by the Application layer. LineTotal is calculated
/// from Quantity and UnitPrice; SQL Server owns the persisted computed column.
/// </summary>
public class QuotationItem
{
    public int QuotationItemId { get; set; }
    public int QuotationId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? DesignNotes { get; set; }
    public decimal? LineTotal { get; set; }
}
