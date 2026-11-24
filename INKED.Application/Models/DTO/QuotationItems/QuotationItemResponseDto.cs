namespace INKED.Application.Models.DTO.QuotationItems;

public class QuotationItemResponseDto
{
    public int QuotationItemId { get; set; }
    public int QuotationId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? DesignNotes { get; set; }

    /// <summary>Quantity x UnitPrice (computed column in SQL Server).</summary>
    public decimal LineTotal { get; set; }
}
