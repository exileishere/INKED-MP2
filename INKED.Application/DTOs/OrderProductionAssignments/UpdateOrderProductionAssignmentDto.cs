using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

// OrderID and StaffID identify the assignment; to reassign, delete and create a new one.
public class UpdateOrderProductionAssignmentDto
{
    /// <summary>Not Started, In Progress, Done, or On Hold.</summary>
    [Required, StringLength(15)]
    public string ProductionStatus { get; set; } = string.Empty;

    public DateOnly? DueDate { get; set; }
}
