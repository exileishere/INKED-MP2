using INKED.Application.Common.Exceptions;
using INKED.Application.DTOs;
using INKED.Application.Interfaces;

namespace INKED.Application.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staff;
    private readonly IProductionTeamRepository _teams;

    public StaffService(IStaffRepository staff, IProductionTeamRepository teams)
    {
        _staff = staff;
        _teams = teams;
    }

    public async Task<IReadOnlyList<StaffResponseDto>> GetAllAsync()
        => await _staff.GetAllAsync();

    public async Task<StaffResponseDto> GetByIdAsync(int id)
        => await GetOrThrowAsync(id);

    public async Task<StaffResponseDto> CreateAsync(CreateStaffDto dto)
    {
        await EnsureTeamExistsAsync(dto.TeamID);

        var data = new StaffResponseDto
        {
            TeamID = dto.TeamID,
            FirstName = Required(dto.FirstName, "FirstName"),
            LastName = Required(dto.LastName, "LastName"),
            ContactNumber = Optional(dto.ContactNumber),
            IsActive = true
        };

        return await _staff.AddAsync(data);
    }

    public async Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto dto)
    {
        var staff = await GetOrThrowAsync(id);
        await EnsureTeamExistsAsync(dto.TeamID);

        staff.TeamID = dto.TeamID;
        staff.FirstName = Required(dto.FirstName, "FirstName");
        staff.LastName = Required(dto.LastName, "LastName");
        staff.ContactNumber = Optional(dto.ContactNumber);
        staff.IsActive = dto.IsActive;

        await _staff.UpdateAsync(staff);
        return staff;
    }

    public async Task DeleteAsync(int id)
    {
        await GetOrThrowAsync(id);
        if (await _staff.HasDependentsAsync(id))
            throw new ConflictException(
                "This staff member cannot be deleted because production assignments or a user account " +
                "still reference them. Set IsActive to false instead.");

        await _staff.DeleteAsync(id);
    }

    // ---- helpers ----
    private async Task<StaffResponseDto> GetOrThrowAsync(int id)
        => await _staff.GetByIdAsync(id)
           ?? throw new NotFoundException($"Staff member with ID {id} was not found.");

    private async Task EnsureTeamExistsAsync(int teamId)
    {
        if (!await _teams.ExistsAsync(teamId))
            throw new BadRequestException($"TeamID {teamId} does not exist in ProductionTeams.");
    }

    private static string Required(string? value, string field)
    {
        var cleaned = value?.Trim();
        if (string.IsNullOrEmpty(cleaned))
            throw new BadRequestException($"{field} is required and cannot be blank.");
        return cleaned;
    }

    private static string? Optional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
