using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class V4
{
    public string? TableName { get; set; }

    public string ConstraintName { get; set; } = null!;

    public string Kind { get; set; } = null!;
}
