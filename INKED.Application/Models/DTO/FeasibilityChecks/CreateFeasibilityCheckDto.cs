using System.ComponentModel.DataAnnotations;

namespace INKED.Application.Models.DTO.FeasibilityChecks;

public class CreateFeasibilityCheckDto
{
    [Range(1, int.MaxValue, ErrorMessage = "InquiryId must be a valid Inquiries.InquiryID.")]
    public int InquiryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "TeamId must be a valid ProductionTeams.TeamID.")]
    public int TeamId { get; set; }

    /// <summary>Pending | Feasible | Not Feasible. Defaults to 'Pending'.</summary>
    [StringLength(15)]
    public string? Result { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    /// <summary>Optional. Defaults to the current date/time.</summary>
    public DateTime? RequestedAt { get; set; }

    /// <summary>Optional. Cannot be earlier than RequestedAt.</summary>
    public DateTime? RespondedAt { get; set; }
}
