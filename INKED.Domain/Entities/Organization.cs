namespace INKED.Domain.Entities;

public class Organization
{
    public int OrganizationId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public string OrganizationType { get; set; } = string.Empty;
}