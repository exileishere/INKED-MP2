using INKED.Application.Common.Exceptions;
using INKED.Application.DTOs;
using INKED.Application.Interfaces;

namespace INKED.Application.Services;

public class ProductionTeamService : IProductionTeamService
{
    private readonly IProductionTeamRepository _teams;

    public ProductionTeamService(IProductionTeamRepository teams) => _teams = teams;

    public async Task<IReadOnlyList<ProductionTeamResponseDto>> GetAllAsync()
        => await _teams.GetAllAsync();

    public async Task<ProductionTeamResponseDto> GetByIdAsync(int id)
        => await GetOrThrowAsync(id);

    public async Task<ProductionTeamResponseDto> CreateAsync(CreateProductionTeamDto dto)
    {
        var name = CleanName(dto.TeamName);
        if (await _teams.NameExistsAsync(name))
            throw new ConflictException($"A production team named '{name}' already exists.");

        return await _teams.AddAsync(new ProductionTeamResponseDto { TeamName = name });
    }

    public async Task<ProductionTeamResponseDto> UpdateAsync(int id, UpdateProductionTeamDto dto)
    {
        var team = await GetOrThrowAsync(id);
        var name = CleanName(dto.TeamName);
        if (await _teams.NameExistsAsync(name, excludeId: id))
            throw new ConflictException($"A production team named '{name}' already exists.");

        team.TeamName = name;
        await _teams.UpdateAsync(team);
        return team;
    }

    public async Task DeleteAsync(int id)
    {
        await GetOrThrowAsync(id);
        if (await _teams.HasDependentsAsync(id))
            throw new ConflictException(
                "This team cannot be deleted because staff members or feasibility checks still reference it.");

        await _teams.DeleteAsync(id);
    }

    // ---- helpers ----
    private async Task<ProductionTeamResponseDto> GetOrThrowAsync(int id)
        => await _teams.GetByIdAsync(id)
           ?? throw new NotFoundException($"Production team with ID {id} was not found.");

    private static string CleanName(string? name)
    {
        var cleaned = name?.Trim();
        if (string.IsNullOrEmpty(cleaned))
            throw new BadRequestException("TeamName is required and cannot be blank.");
        return cleaned;
    }
}
