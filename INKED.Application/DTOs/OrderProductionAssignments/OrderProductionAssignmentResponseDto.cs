namespace INKED.Application.DTOs;

public class OrderProductionAssignmentResponseDto
{
    public int AssignmentID { get; set; }
    public int OrderID { get; set; }
    public int StaffID { get; set; }
    public DateTime DateAssigned { get; set; }
    public string ProductionStatus { get; set; } = string.Empty;
    public DateTime? StatusUpdatedAt { get; set; }
    public DateOnly? DueDate { get; set; }
}
