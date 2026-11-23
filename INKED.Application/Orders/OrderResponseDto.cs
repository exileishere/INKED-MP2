// INKED MP2 | Table: Orders | Layer: Application / Response DTO
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Orders;

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public int QuotationId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateOnly? RequiredDate { get; set; }
    public bool IsLocked { get; set; }
}
