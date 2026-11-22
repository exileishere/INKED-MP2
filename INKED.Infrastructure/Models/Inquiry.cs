using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class Inquiry
{
    public int InquiryId { get; set; }

    public int CustomerId { get; set; }

    public int? HandledByUserId { get; set; }

    public DateTime DateReceived { get; set; }

    public string InquiryChannel { get; set; } = null!;

    public string? InquiryDetails { get; set; }

    public bool NeedsDesignHelp { get; set; }

    public DateTime? FirstResponseAt { get; set; }

    public string Outcome { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<FeasibilityCheck> FeasibilityChecks { get; set; } = new List<FeasibilityCheck>();

    public virtual UserAccount? HandledByUser { get; set; }

    public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}
