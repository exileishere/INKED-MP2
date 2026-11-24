using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.Inquiries;

public class CreateInquiryDto
{
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be a valid Customers.CustomerID.")]
    public int CustomerId { get; set; }

    /// <summary>Optional. Must exist in UserAccounts when supplied.</summary>
    [Range(1, int.MaxValue)]
    public int? HandledByUserId { get; set; }

    /// <summary>Optional. Defaults to the current date/time.</summary>
    public DateTime? DateReceived { get; set; }

    /// <summary>Inquiry Form | Facebook | Instagram | Shopee | TikTok | Other. Defaults to 'Inquiry Form'.</summary>
    [StringLength(30)]
    public string? InquiryChannel { get; set; }

    [StringLength(1000)]
    public string? InquiryDetails { get; set; }

    public bool NeedsDesignHelp { get; set; }

    /// <summary>Optional. Cannot be earlier than DateReceived.</summary>
    public DateTime? FirstResponseAt { get; set; }

    /// <summary>Open | Quoted | Converted | Not Pursued. Defaults to 'Open'.</summary>
    [StringLength(20)]
    public string? Outcome { get; set; }
}
