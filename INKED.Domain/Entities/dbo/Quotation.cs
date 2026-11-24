namespace INKED.Domain.Entities.dbo;

/// <summary>
/// Domain representation used by the Application layer. EF Core continues to map
/// the database-first entity in INKED.Infrastructure.Models.Quotation.
/// </summary>
public class Quotation
{
    public int QuotationId { get; set; }
    public int InquiryId { get; set; }
    public int PreparedByUserId { get; set; }
    public DateTime DateIssued { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
