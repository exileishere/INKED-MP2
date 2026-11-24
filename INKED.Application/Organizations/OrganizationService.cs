using INKED.Application.DTOs.Organizations;

namespace INKED.Application.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;

    public OrganizationService(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    public Task<List<OrganizationResponseDto>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<OrganizationResponseDto?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public async Task<OrganizationResponseDto> CreateAsync(CreateOrganizationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OrganizationName))
            throw new ArgumentException("Organization name is required.");

        if (string.IsNullOrWhiteSpace(dto.OrganizationType))
            throw new ArgumentException("Organization type is required.");

        if (await _repository.OrganizationNameExistsAsync(dto.OrganizationName))
            throw new ArgumentException("Organization name already exists.");

        return await _repository.AddAsync(dto);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrganizationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OrganizationName))
            throw new ArgumentException("Organization name is required.");

        if (string.IsNullOrWhiteSpace(dto.OrganizationType))
            throw new ArgumentException("Organization type is required.");

        if (await _repository.OrganizationNameExistsAsync(dto.OrganizationName, id))
            throw new ArgumentException("Organization name already exists.");

        return await _repository.UpdateAsync(id, dto);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }
}