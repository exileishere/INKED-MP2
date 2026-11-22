using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public int TeamId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OrderProductionAssignment> OrderProductionAssignments { get; set; } = new List<OrderProductionAssignment>();

    public virtual ProductionTeam Team { get; set; } = null!;

    public virtual UserAccount? UserAccount { get; set; }
}
