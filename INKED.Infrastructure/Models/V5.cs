using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class V5
{
    public string? TableName { get; set; }

    public string? ConstraintName { get; set; }

    public string Kind { get; set; } = null!;
}
