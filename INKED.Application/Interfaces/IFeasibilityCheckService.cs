using INKED.Application.Models.DTO.FeasibilityChecks;

namespace INKED.Application.Interfaces;

public interface IFeasibilityCheckService
{
    Task<IReadOnlyList<FeasibilityCheckResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<FeasibilityCheckResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<FeasibilityCheckResponseDto> CreateAsync(CreateFeasibilityCheckDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateFeasibilityCheckDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
