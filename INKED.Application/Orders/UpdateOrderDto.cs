// INKED MP2 | Table: Orders | Layer: Application / Update DTO
// Owner: Rashel Maxine Baquirin (Billing module)
namespace INKED.Application.Orders;

public class UpdateOrderDto
{
    /// <summary>Customer deadline (send null to clear it). Cannot be earlier than the order date.</summary>
    public DateOnly? RequiredDate { get; set; }

    /// <summary>Optional. Set to true to lock the order. A locked order cannot be unlocked or changed.
    /// Omit to keep the current value. The order is also locked automatically when a Downpayment or Full payment is recorded.</summary>
    public bool? IsLocked { get; set; }
}
