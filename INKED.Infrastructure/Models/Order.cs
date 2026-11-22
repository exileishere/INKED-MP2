using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int QuotationId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateOnly? RequiredDate { get; set; }

    public bool IsLocked { get; set; }

    public virtual ICollection<DeliveryRecord> DeliveryRecords { get; set; } = new List<DeliveryRecord>();

    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderProductionAssignment> OrderProductionAssignments { get; set; } = new List<OrderProductionAssignment>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Quotation Quotation { get; set; } = null!;

    public virtual ICollection<SampleApproval> SampleApprovals { get; set; } = new List<SampleApproval>();
}
