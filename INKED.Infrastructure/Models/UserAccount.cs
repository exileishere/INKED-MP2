using System;
using System.Collections.Generic;

namespace INKED.Infrastructure.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public int? StaffId { get; set; }

    public int? CustomerId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();

    public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();

    public virtual Staff? Staff { get; set; }
}
