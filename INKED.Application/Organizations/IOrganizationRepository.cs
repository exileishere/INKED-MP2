using INKED.Application.DTOs.Organizations;

namespace INKED.Application.Organizations;

public interface IOrganizationRepository
{
    Task<List<OrganizationResponseDto>> GetAllAsync();

    Task<OrganizationResponseDto?> GetByIdAsync(int id);

    Task<OrganizationResponseDto> AddAsync(CreateOrganizationDto dto);

    Task<bool> UpdateAsync(int id, UpdateOrganizationDto dto);

    Task<bool> DeleteAsync(int id);

    Task<bool> OrganizationNameExistsAsync(string organizationName, int? excludeOrganizationId = null);
}