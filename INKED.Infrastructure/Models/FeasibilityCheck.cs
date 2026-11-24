namespace INKED.Domain.Entities.dbo;

/// <summary>
/// Domain representation used by the Application layer. EF Core continues to map
/// the database-first entity in INKED.Infrastructure.Models.FeasibilityCheck.
/// </summary>
public class FeasibilityCheck
{
    public int FeasibilityId { get; set; }
    public int InquiryId { get; set; }
    public int TeamId { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
