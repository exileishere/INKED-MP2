using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class ProductionTeam
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public virtual ICollection<FeasibilityCheck> FeasibilityChecks { get; set; } = new List<FeasibilityCheck>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
