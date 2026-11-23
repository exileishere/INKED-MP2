// INKED MP2 | Table: Payments | Layer: Application / Update DTO
// Owner: Rashel Maxine Baquirin (Billing module)
using System.ComponentModel.DataAnnotations;
using INKED.Application.Billing;

namespace INKED.Application.Payments;

/// <summary>OrderId is not updatable: a payment always belongs to the order it was received for.</summary>
public class UpdatePaymentDto
{
    [Required]
    public DateTime? PaymentDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "99999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [DecimalPlaces(2)]
    public decimal? Amount { get; set; }

    [Required, StringLength(15), OneOf("Downpayment", "Balance", "Full")]
    public string PaymentType { get; set; } = string.Empty;

    [Required, StringLength(20), OneOf("Cash", "GCash", "Bank Transfer", "Other")]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ReferenceNo { get; set; }
}
