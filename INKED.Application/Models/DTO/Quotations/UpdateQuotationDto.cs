using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.Quotations;

/// <summary>InquiryId and DateIssued are fixed once a quotation is created.</summary>
public class UpdateQuotationDto
{
    [Range(1, int.MaxValue)]
    public int PreparedByUserId { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal TotalAmount { get; set; }

    [Required, StringLength(15)]
    public string Status { get; set; } = string.Empty;
}
