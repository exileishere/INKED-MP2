// INKED MP2 | Table: Invoices | Layer: Application / Update DTO
// Owner: Rashel Maxine Baquirin (Billing module)
using System.ComponentModel.DataAnnotations;
using INKED.Application.Billing;

namespace INKED.Application.Invoices;

/// <summary>OrderId is not updatable: an invoice always belongs to the order it was issued for.</summary>
public class UpdateInvoiceDto
{
    [Required, StringLength(20)]
    public string InvoiceNo { get; set; } = string.Empty;

    [Required]
    public DateOnly? InvoiceDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "99999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [DecimalPlaces(2)]
    public decimal? TotalAmount { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "1.00", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [DecimalPlaces(2)]
    public decimal? DownpaymentRate { get; set; }

    [Required, StringLength(15), OneOf("Draft", "Sent", "Confirmed", "Cancelled")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Required when Status is Confirmed. If omitted for a Confirmed invoice, the current time is used.</summary>
    public DateTime? ConfirmedAt { get; set; }
}
