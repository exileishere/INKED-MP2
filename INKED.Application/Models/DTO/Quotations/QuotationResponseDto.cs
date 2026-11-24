namespace INKED.Application.Models.DTO.Quotations;

public class QuotationResponseDto
{
    public int QuotationId { get; set; }
    public int InquiryId { get; set; }
    public int PreparedByUserId { get; set; }
    public DateTime DateIssued { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
