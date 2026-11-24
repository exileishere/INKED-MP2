using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.Quotations;

public class CreateQuotationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "InquiryId must be a valid Inquiries.InquiryID.")]
    public int InquiryId { get; set; }

    /// <summary>Required by the MP1 schema (PreparedByUserID is NOT NULL).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "PreparedByUserId must be a valid UserAccounts.UserID.")]
    public int PreparedByUserId { get; set; }

    /// <summary>Optional. Defaults to the current date/time.</summary>
    public DateTime? DateIssued { get; set; }

    /// <summary>DECIMAL(12,2), cannot be negative. Defaults to 0.</summary>
    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal TotalAmount { get; set; }

    /// <summary>Draft | Sent | Accepted | Rejected | Expired. Defaults to 'Draft'.</summary>
    [StringLength(15)]
    public string? Status { get; set; }
}
