namespace INKED.Application.DTOs.Organizations;

public class OrganizationResponseDto
{
    public int OrganizationId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public string OrganizationType { get; set; } = string.Empty;
}