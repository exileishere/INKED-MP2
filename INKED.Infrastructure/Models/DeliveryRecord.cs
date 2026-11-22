using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class DeliveryRecord
{
    public int DeliveryId { get; set; }

    public int OrderId { get; set; }

    public string DeliveryType { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string DeliveryAddress { get; set; } = null!;

    public string? CourierName { get; set; }

    public string? TrackingNo { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public string DeliveryStatus { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
