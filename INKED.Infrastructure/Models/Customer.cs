using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? OrganizationId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly DateRegistered { get; set; }

    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();

    public virtual Organization? Organization { get; set; }

    public virtual UserAccount? UserAccount { get; set; }
}
