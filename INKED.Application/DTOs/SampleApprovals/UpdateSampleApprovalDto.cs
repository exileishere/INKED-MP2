using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

// OrderID, RevisionNo and DateSent identify the sample round, so they cannot be changed.
public class UpdateSampleApprovalDto
{
    [StringLength(260)]
    public string? SamplePhotoPath { get; set; }

    /// <summary>Pending, Approved, or Revision Requested.</summary>
    [Required, StringLength(20)]
    public string ApprovalStatus { get; set; } = string.Empty;

    public DateTime? DateResponded { get; set; }

    [StringLength(500)]
    public string? CustomerRemarks { get; set; }
}
