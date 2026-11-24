namespace INKED.Application.Models.DTO.FeasibilityChecks;

public class FeasibilityCheckResponseDto
{
    public int FeasibilityId { get; set; }
    public int InquiryId { get; set; }
    public int TeamId { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
