namespace INKED.Application.DTOs;

public class StaffResponseDto
{
    public int StaffID { get; set; }
    public int TeamID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public bool IsActive { get; set; }
}
