using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

public class CreateStaffDto
{
    [Range(1, int.MaxValue)]
    public int TeamID { get; set; }

    [Required, StringLength(60)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? ContactNumber { get; set; }
    // IsActive is not accepted on create: new staff are active (DB default = 1).
}
