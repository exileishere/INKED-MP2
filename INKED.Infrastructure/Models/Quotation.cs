using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class Quotation
{
    public int QuotationId { get; set; }

    public int InquiryId { get; set; }

    public int PreparedByUserId { get; set; }

    public DateTime DateIssued { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public virtual Inquiry Inquiry { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual UserAccount PreparedByUser { get; set; } = null!;

    public virtual ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}
