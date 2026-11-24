namespace INKED.Domain.Entities.dbo;

/// <summary>
/// Domain representation used by the Application layer. EF Core continues to map
/// the database-first entity in INKED.Infrastructure.Models.Inquiry.
/// </summary>
public class Inquiry
{
    public int InquiryId { get; set; }
    public int CustomerId { get; set; }
    public int? HandledByUserId { get; set; }
    public DateTime DateReceived { get; set; }
    public string InquiryChannel { get; set; } = string.Empty;
    public string? InquiryDetails { get; set; }
    public bool NeedsDesignHelp { get; set; }
    public DateTime? FirstResponseAt { get; set; }
    public string Outcome { get; set; } = string.Empty;
}
