using System.ComponentModel.DataAnnotations;

namespace INKED.Application.DTOs;

public class CreateOrderProductionAssignmentDto
{
    [Range(1, int.MaxValue)]
    public int OrderID { get; set; }

    [Range(1, int.MaxValue)]
    public int StaffID { get; set; }

    /// <summary>Not Started, In Progress, Done, or On Hold. Defaults to Not Started.</summary>
    [StringLength(15)]
    public string? ProductionStatus { get; set; }

    public DateOnly? DueDate { get; set; }
}
