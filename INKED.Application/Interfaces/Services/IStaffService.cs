using INKED.Application.DTOs;

namespace INKED.Application.Interfaces;

public interface IStaffService
{
    Task<IReadOnlyList<StaffResponseDto>> GetAllAsync();
    Task<StaffResponseDto> GetByIdAsync(int id);
    Task<StaffResponseDto> CreateAsync(CreateStaffDto dto);
    Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto dto);
    Task DeleteAsync(int id);
}
