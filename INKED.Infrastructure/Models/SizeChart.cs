using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class SizeChart
{
    public int SizeChartId { get; set; }

    public int QuotationItemId { get; set; }

    public string Size { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual QuotationItem QuotationItem { get; set; } = null!;
}
