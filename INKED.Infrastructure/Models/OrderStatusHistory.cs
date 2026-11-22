using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class OrderStatusHistory
{
    public int HistoryId { get; set; }

    public int OrderId { get; set; }

    public int StatusId { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? Remarks { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;
}
