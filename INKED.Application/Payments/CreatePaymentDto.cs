// INKED MP2 | Table: Payments | Layer: Application / Create DTO
// Owner: Rashel Maxine Baquirin (Billing module)
using System.ComponentModel.DataAnnotations;
using INKED.Application.Billing;

namespace INKED.Application.Payments;

public class CreatePaymentDto
{
    [Required]
    public int? OrderId { get; set; }

    /// <summary>Optional. Defaults to now.</summary>
    public DateTime? PaymentDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "99999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [DecimalPlaces(2)]
    public decimal? Amount { get; set; }

    /// <summary>Downpayment, Balance or Full. Only one of each type per order.</summary>
    [Required, StringLength(15), OneOf("Downpayment", "Balance", "Full")]
    public string PaymentType { get; set; } = string.Empty;

    [Required, StringLength(20), OneOf("Cash", "GCash", "Bank Transfer", "Other")]
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>Optional receipt or transaction number.</summary>
    [StringLength(50)]
    public string? ReferenceNo { get; set; }
}
