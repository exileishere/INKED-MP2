// INKED MP2 | Table: Payments | Layer: Application / Response DTO
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Payments;

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNo { get; set; }
}
