using INKED.Application.DTOs.Organizations;
using INKED.Application.Organizations;
using INKED.Infrastructure.Data;
using INKED.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace INKED.Infrastructure.Organizations;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly INKEDDbContext _context;

    public OrganizationRepository(INKEDDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationResponseDto>> GetAllAsync()
    {
        return await _context.Organizations
            .Select(o => new OrganizationResponseDto
            {
                OrganizationId = o.OrganizationId,
                OrganizationName = o.OrganizationName,
                OrganizationType = o.OrganizationType
            })
            .ToListAsync();
    }

    public async Task<OrganizationResponseDto?> GetByIdAsync(int id)
    {
        var organization = await _context.Organizations.FindAsync(id);

        if (organization == null)
            return null;

        return new OrganizationResponseDto
        {
            OrganizationId = organization.OrganizationId,
            OrganizationName = organization.OrganizationName,
            OrganizationType = organization.OrganizationType
        };
    }

    public async Task<OrganizationResponseDto> AddAsync(CreateOrganizationDto dto)
    {
        var organization = new Organization
        {
            OrganizationName = dto.OrganizationName,
            OrganizationType = dto.OrganizationType
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        return new OrganizationResponseDto
        {
            OrganizationId = organization.OrganizationId,
            OrganizationName = organization.OrganizationName,
            OrganizationType = organization.OrganizationType
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrganizationDto dto)
    {
        var organization = await _context.Organizations.FindAsync(id);

        if (organization == null)
            return false;

        organization.OrganizationName = dto.OrganizationName;
        organization.OrganizationType = dto.OrganizationType;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var organization = await _context.Organizations.FindAsync(id);

        if (organization == null)
            return false;

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> OrganizationNameExistsAsync(
        string organizationName,
        int? excludeOrganizationId = null)
    {
        return await _context.Organizations.AnyAsync(o =>
            o.OrganizationName == organizationName &&
            (excludeOrganizationId == null ||
             o.OrganizationId != excludeOrganizationId));
    }
}