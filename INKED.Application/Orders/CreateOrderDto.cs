// INKED MP2 | Table: Orders | Layer: Application / Create DTO
// Owner: Rashel Maxine Baquirin (Billing module)
using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Orders;

public class CreateOrderDto
{
    /// <summary>The accepted quotation that becomes this order (one quotation = one order).</summary>
    [Required]
    public int? QuotationId { get; set; }

    /// <summary>Optional customer deadline. Cannot be earlier than the order date (today).</summary>
    public DateOnly? RequiredDate { get; set; }
}
