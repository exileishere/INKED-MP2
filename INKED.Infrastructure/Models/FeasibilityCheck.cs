using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class FeasibilityCheck
{
    public int FeasibilityId { get; set; }

    public int InquiryId { get; set; }

    public int TeamId { get; set; }

    public string Result { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? RespondedAt { get; set; }

    public virtual Inquiry Inquiry { get; set; } = null!;

    public virtual ProductionTeam Team { get; set; } = null!;
}
