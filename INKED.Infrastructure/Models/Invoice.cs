using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int OrderId { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public DateOnly InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal DownpaymentRate { get; set; }

    public decimal? DownpaymentAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? ConfirmedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
