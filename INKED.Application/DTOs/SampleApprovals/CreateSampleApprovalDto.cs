using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

public class CreateSampleApprovalDto
{
    [Range(1, int.MaxValue)]
    public int OrderID { get; set; }

    /// <summary>Optional. If omitted, the next revision number for the order is used.</summary>
    [Range(1, 255)]
    public byte? RevisionNo { get; set; }

    [StringLength(260)]
    public string? SamplePhotoPath { get; set; }

    /// <summary>Optional. Defaults to the current date/time.</summary>
    public DateTime? DateSent { get; set; }

    /// <summary>Pending, Approved, or Revision Requested. Defaults to Pending.</summary>
    [StringLength(20)]
    public string? ApprovalStatus { get; set; }

    /// <summary>Must be empty when Pending. Defaults to now when Approved / Revision Requested.</summary>
    public DateTime? DateResponded { get; set; }

    [StringLength(500)]
    public string? CustomerRemarks { get; set; }
}
