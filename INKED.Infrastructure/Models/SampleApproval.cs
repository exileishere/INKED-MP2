using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class SampleApproval
{
    public int SampleApprovalId { get; set; }

    public int OrderId { get; set; }

    public byte RevisionNo { get; set; }

    public string? SamplePhotoPath { get; set; }

    public DateTime DateSent { get; set; }

    public string ApprovalStatus { get; set; } = null!;

    public DateTime? DateResponded { get; set; }

    public string? CustomerRemarks { get; set; }

    public virtual Order Order { get; set; } = null!;
}
