// INKED MP2 | Table: Invoices | Layer: Application / Response DTO
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Invoices;

public class InvoiceResponseDto
{
    public int InvoiceId { get; set; }
    public int OrderId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DownpaymentRate { get; set; }
    /// <summary>Computed by SQL Server: TotalAmount x DownpaymentRate.</summary>
    public decimal? DownpaymentAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ConfirmedAt { get; set; }
}
