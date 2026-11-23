using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface IStaffRepository
{
    Task<IReadOnlyList<StaffResponseDto>> GetAllAsync();
    Task<StaffResponseDto?> GetByIdAsync(int id);
    Task<StaffResponseDto> AddAsync(StaffResponseDto staff);   // StaffID is ignored
    Task UpdateAsync(StaffResponseDto staff);
    Task DeleteAsync(int id);

    /// <summary>True if OrderProductionAssignments or UserAccounts still reference the staff member.</summary>
    Task<bool> HasDependentsAsync(int id);
}
