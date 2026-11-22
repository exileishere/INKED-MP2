using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class QuotationItem
{
    public int QuotationItemId { get; set; }

    public int QuotationId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string? DesignNotes { get; set; }

    public decimal? LineTotal { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Quotation Quotation { get; set; } = null!;

    public virtual ICollection<SizeChart> SizeCharts { get; set; } = new List<SizeChart>();
}
