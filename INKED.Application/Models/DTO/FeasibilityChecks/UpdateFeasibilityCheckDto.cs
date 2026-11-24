using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.FeasibilityChecks;

/// <summary>InquiryId and RequestedAt are fixed once a check is created.</summary>
public class UpdateFeasibilityCheckDto
{
    [Range(1, int.MaxValue)]
    public int TeamId { get; set; }

    [Required, StringLength(15)]
    public string Result { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Remarks { get; set; }

    public DateTime? RespondedAt { get; set; }
}
