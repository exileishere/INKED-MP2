namespace INKED.Application.DTOs.Organizations;

public class CreateOrganizationDto
{
    public string OrganizationName { get; set; } = string.Empty;

    public string OrganizationType { get; set; } = string.Empty;
}