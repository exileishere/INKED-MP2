namespace INKED.Application.DTOs;

public class SampleApprovalResponseDto
{
    public int SampleApprovalID { get; set; }
    public int OrderID { get; set; }
    public byte RevisionNo { get; set; }
    public string? SamplePhotoPath { get; set; }
    public DateTime DateSent { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public DateTime? DateResponded { get; set; }
    public string? CustomerRemarks { get; set; }
}
