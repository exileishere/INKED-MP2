// INKED MP2 | Table: Invoices | Layer: Application / Create DTO
// Owner: Rashel Maxine Baquirin (Billing module)
using System.ComponentModel.DataAnnotations;
using INKED.Application.Billing;

namespace INKED.Application.Invoices;

public class CreateInvoiceDto
{
    /// <summary>The order being billed. Each order can have only one invoice.</summary>
    [Required]
    public int? OrderId { get; set; }

    /// <summary>Printed invoice number. Must be unique.</summary>
    [Required, StringLength(20)]
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>Optional. Defaults to today.</summary>
    public DateOnly? InvoiceDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "99999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [DecimalPlaces(2)]
    public decimal? TotalAmount { get; set; }

    /// <summary>Optional. Defaults to 0.60 (60% downpayment). Must be greater than 0 and at most 1.</summary>
    [Range(typeof(decimal), "0.01", "1.00", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [DecimalPlaces(2)]
    public decimal? DownpaymentRate { get; set; }

    /// <summary>Optional. Defaults to "Sent".</summary>
    [StringLength(15), OneOf("Draft", "Sent", "Confirmed", "Cancelled")]
    public string? Status { get; set; }

    /// <summary>Required when Status is Confirmed. If omitted for a Confirmed invoice, the current time is used.</summary>
    public DateTime? ConfirmedAt { get; set; }
}
