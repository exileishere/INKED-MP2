using INKED.Application.DTOs.Organizations;

namespace INKED.Application.Organizations;

public interface IOrganizationService
{
    Task<List<OrganizationResponseDto>> GetAllAsync();

    Task<OrganizationResponseDto?> GetByIdAsync(int id);

    Task<OrganizationResponseDto> CreateAsync(CreateOrganizationDto dto);

    Task<bool> UpdateAsync(int id, UpdateOrganizationDto dto);

    Task<bool> DeleteAsync(int id);
}