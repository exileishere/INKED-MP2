using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class OrderProductionAssignment
{
    public int AssignmentId { get; set; }

    public int OrderId { get; set; }

    public int StaffId { get; set; }

    public DateTime DateAssigned { get; set; }

    public string ProductionStatus { get; set; } = null!;

    public DateTime? StatusUpdatedAt { get; set; }

    public DateOnly? DueDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
