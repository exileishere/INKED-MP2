using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class ProductCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string ProductionType { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
